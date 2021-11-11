using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleApiWebApp.Constants;
using SampleApiWebApp.Data;
using SampleApiWebApp.Data.Queries;
using SampleApiWebApp.Domain;

namespace SampleApiWebApp.Controllers.Teams
{
    [ApiController]
    [Route("[controller]")]
    public sealed class TeamsController : Controller
    {
        private readonly IDbContextFactory<DatabaseContext> contextFactory;
        private readonly IMapper mapper;

        public TeamsController(IDbContextFactory<DatabaseContext> contextFactory, IMapper mapper)
        {
            this.contextFactory = contextFactory;
            this.mapper = mapper;
        }

        [HttpGet]
        [Produces(ContentTypes.ApplicationJson)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Team>))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            using (var context = this.contextFactory.CreateDbContext())
            {
                var teams = await context
                    .Set<Domain.Team>()
                    .ToListAsync(cancellationToken);

                var teamsResponse = this.mapper.Map<IEnumerable<Team>>(teams);

                return this.Ok(teamsResponse);
            }
        }

        [HttpGet("{id}")]
        [Consumes(ContentTypes.ApplicationJson)]
        [Produces(ContentTypes.ApplicationJson)]
        [ProducesResponseType(200, Type = typeof(Team))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOne([FromRoute] long id, CancellationToken cancellationToken = default)
        {
            using (var context = this.contextFactory.CreateDbContext())
            {
                var team = await LookupTeamId(context, id, cancellationToken);

                if (team == null)
                {
                    return this.NotFound();
                }

                var teamResponse = this.mapper.Map<Team>(team);

                return this.Ok(teamResponse);
            }
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
            using (var context = this.contextFactory.CreateDbContext())
            {
                var team = await GenerateAndValidateDomainEntity(context, request, cancellationToken);

                context.Set<Domain.Team>()
                    .Add(team);

                await context.SaveChangesAsync(cancellationToken);

                return this.Ok(team.Id);
            }
        }

        [HttpPut("{id}")]
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
            using (var context = this.contextFactory.CreateDbContext())
            {
                var team = await LookupTeamId(context, id, cancellationToken);

                if (team == null)
                {
                    return this.NotFound();
                }

                await BindToDomainEntityAndValidate(context, team, request, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                return this.Ok();
            }
        }

        private static void ThrowTeamWithSameNameException(ITeam request)
        {
            var error = new ValidationFailure(nameof(request.Name), string.Format(Domain.Team.ErrorMessages.NameNotUniqueFormat, request.Name));
            throw new ValidationException(new ValidationFailure[] { error });
        }

        private static async Task<Domain.Team> LookupTeamId(
            DatabaseContext context,
            long id,
            CancellationToken cancellationToken)
        {
            return await context
                .Set<Domain.Team>()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        private static async Task<Domain.Team[]> LookupTeamByName(
            DatabaseContext context,
            ITeam request,
            CancellationToken cancellationToken)
        {
            return await context
                .Set<Domain.Team>()
                .GetTeamsByName(request.Name)
                .ToArrayAsync(cancellationToken);
        }

        private static async Task<Domain.Team> GenerateAndValidateDomainEntity(
            DatabaseContext context,
            TeamRequest request,
            CancellationToken cancellationToken)
        {
            var teamName = request.Name.Trim();
            var teamsWithSameName = await LookupTeamByName(context, request, cancellationToken);

            if (teamsWithSameName.Any())
            {
                ThrowTeamWithSameNameException(request);
            }

            return Domain.Team.CreateTeam(teamName);
        }

        private static async Task BindToDomainEntityAndValidate(
            DatabaseContext context,
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

            var teamsWithSameName = await context
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
