using System.Collections.Generic;
using EntityManagement.Core;
using FluentValidation;
using SampleApiWebApp.Domain.Validators;

namespace SampleApiWebApp.Domain
{
    public class Team : BaseEntity<long>, ITeam
    {
        public string Name { get; protected set; }

        public IList<Player> Players { get; protected set; }

        public static Team CreateTeam(string teamName)
        {
            var team = new Team { Name = teamName };

            team.ApplyTrackingData();

            ValidateTeam(team);

            return team;
        }

        public static void ValidateTeam(Team team)
        {
            var validator = new TeamValidator();

            var result = validator.Validate(team);

            if (result.IsValid) return;

            throw new ValidationException(result.Errors);
        }

        public void ChangeName(string newName)
        {
            this.Name = newName;

            this.ApplyTrackingData();

            ValidateTeam(this);
        }

        public static class FieldMaxLenghts
        {
            public const int Name = 50;
        }

        public static class ErrorMessages
        {
            public const string NameNotUniqueFormat = "Team Name '{0}' is not unqiue";
        }
    }
}
