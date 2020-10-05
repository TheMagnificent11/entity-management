using SampleApiWebApp.Domain;

namespace SampleApiWebApp.Controllers.Teams
{
    public class TeamRequest : ITeam
    {
        public string Name { get; set; }
    }
}
