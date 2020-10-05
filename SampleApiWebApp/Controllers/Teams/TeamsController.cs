using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityManagement;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleApiWebApp.Constants;
using SampleApiWebApp.Data.Queries;
using SampleApiWebApp.Domain;

namespace SampleApiWebApp.Controllers.Teams
{
    [ApiController]
    [Route("[controller]")]
    public sealed class TeamsController : Controller
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IMapper mapper;

        public TeamsController(IDatabaseContext databaseContext, IMapper mapper)
        {
            this.databaseContext = databaseContext;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("{id}")]
        [Consumes(ContentTypes.ApplicationJson)]
        [Produces(ContentTypes.ApplicationJson)]
        [ProducesResponseType(200, Type = typeof(Team))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOne([FromRoute] long id, CancellationToken cancellationToken = default)
        {
            var team = await this.LookupTeamId(id, cancellationToken);

            if (team == null)
            {
                return this.NotFound();
            }

            var teamResponse = this.mapper.Map<Team>(team);

            return this.Ok(teamResponse);
        }

        [HttpPost]
        [Consumes(ContentTypes.ApplicationJson)]
        [Produces(ContentTypes.ApplicationJson)]
        [ProducesResponseType(200, Type = typeof(long))]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> Post(
            [FromBody] TeamRequest request,
            CancellationToken cancellationToken = default)
        {
            var team = await this.GenerateAndValidateDomainEntity(request, cancellationToken);

            this.databaseContext
                .EntitySet<Domain.Team>()
                .Add(team);

            await this.databaseContext.SaveChangesAsync(cancellationToken);

            return this.Ok(team.Id);
        }

        [HttpPut]
        [Route("{id}")]
        [Consumes(ContentTypes.ApplicationJson)]
        [Produces(ContentTypes.ApplicationJson)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(
            [FromRoute] long id,
            [FromBody] TeamRequest request,
            CancellationToken cancellationToken = default)
        {
            var team = await this.LookupTeamId(id, cancellationToken);

            if (team == null)
            {
                return this.NotFound();
            }

            await this.BindToDomainEntityAndValidate(team, request, cancellationToken);

            await this.databaseContext.SaveChangesAsync(cancellationToken);

            return this.Ok();
        }

        private static void ThrowTeamWithSameNameException(ITeam request)
        {
            var error = new ValidationFailure(nameof(request.Name), string.Format(Domain.Team.ErrorMessages.NameNotUniqueFormat, request.Name));
            throw new ValidationException(new ValidationFailure[] { error });
        }

        private async Task<Domain.Team> LookupTeamId(long id, CancellationToken cancellationToken)
        {
            return await this.databaseContext
                .EntitySet<Domain.Team>()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        private async Task<Domain.Team[]> LookupTeamByName(ITeam request, CancellationToken cancellationToken)
        {
            return await this.databaseContext
                .EntitySet<Domain.Team>()
                .GetTeamsByName(request.Name)
                .ToArrayAsync(cancellationToken);
        }

        private async Task<Domain.Team> GenerateAndValidateDomainEntity(
            TeamRequest request,
            CancellationToken cancellationToken)
        {
            var teamName = request.Name.Trim();
            var teamsWithSameName = await this.LookupTeamByName(request, cancellationToken);

            if (teamsWithSameName.Any())
            {
                ThrowTeamWithSameNameException(request);
            }

            return Domain.Team.CreateTeam(teamName);
        }

        private async Task BindToDomainEntityAndValidate(
            Domain.Team domainEntity,
            TeamRequest request,
            CancellationToken cancellationToken)
        {
            if (domainEntity == null)
            {
                throw new ArgumentNullException(nameof(domainEntity));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var teamsWithSameName = await this.databaseContext
                .EntitySet<Domain.Team>()
                .GetTeamsByName(request.Name)
                .ToArrayAsync(cancellationToken);

            if (teamsWithSameName.Any(i => i.Id != domainEntity.Id))
            {
                ThrowTeamWithSameNameException(request);
            }

            domainEntity.ChangeName(request.Name);
        }
    }
}
