using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityManagement;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleApiWebApp.Data.Queries;
using SampleApiWebApp.Infrastructure;
using Serilog;

namespace SampleApiWebApp.Controllers.Teams.Post
{
    public sealed class PostTeamCommandHandler : IRequestHandler<PostTeamCommand, OperationResult<long>>
    {
        private readonly IDatabaseContext databaseContext;
        private readonly ILogger logger;

        public PostTeamCommandHandler(IDatabaseContext databaseContext, ILogger logger)
        {
            this.databaseContext = databaseContext;
            this.logger = logger.ForContext<PostTeamCommandHandler>();
        }

        public async Task<OperationResult<long>> Handle(PostTeamCommand request, CancellationToken cancellationToken)
        {
            using (this.logger.BeginTimedOperation(nameof(PostTeamCommandHandler)))
            {
                var team = await this.GenerateAndValidateDomainEntity(request, cancellationToken);

                this.databaseContext
                    .EntitySet<Domain.Team>()
                    .Add(team);

                await this.databaseContext.SaveChangesAsync(cancellationToken);

                return OperationResult.Success(team.Id);
            }
        }

        private async Task<Domain.Team> GenerateAndValidateDomainEntity(
            PostTeamCommand request,
            CancellationToken cancellationToken)
        {
            var teamName = request.Name.Trim();

            var teamsWithSameName = await this.databaseContext
                .EntitySet<Domain.Team>()
                .GetTeamsByName(request.Name)
                .ToArrayAsync(cancellationToken);

            if (teamsWithSameName.Any())
            {
                var error = new ValidationFailure(nameof(request.Name), string.Format(Domain.Team.ErrorMessages.NameNotUniqueFormat, teamName));
                this.logger.Information("Validation failed: a Team with the name {TeamName} already exists", request.Name);
                throw new ValidationException(new ValidationFailure[] { error });
            }

            return Domain.Team.CreateTeam(teamName);
        }
    }
}
