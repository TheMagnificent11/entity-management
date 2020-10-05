using MediatR;
using SampleApiWebApp.Domain;
using SampleApiWebApp.Infrastructure;

namespace SampleApiWebApp.Controllers.Teams.Post
{
    public class PostTeamCommand : ITeam, IRequest<OperationResult<long>>
    {
        public PostTeamCommand(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
