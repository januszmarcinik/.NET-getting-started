using System;
using System.Collections.Generic;
using System.Linq;
using NETCore3.Entities;

namespace NETCore3.Services
{
    internal class TeamMembersService : ITeamMembersService
    {
        private readonly List<TeamMember> _teamMembers;

        public TeamMembersService(params TeamMember[] initTeamMembers)
        {
            _teamMembers = new List<TeamMember>(initTeamMembers);
        }

        public TeamMember GetById(Guid id)
        {
            var teamMember = _teamMembers.SingleOrDefault(x => x.Id == id);
            if (teamMember == null)
            {
                throw new InvalidOperationException($"Team member with given id '{id}' does not exist.");
            }

            return teamMember;
        }

        public IEnumerable<TeamMember> GetAll() =>
            _teamMembers
                .OrderBy(x => x.Id)
                .ToList();

        public Guid Add(TeamMember teamMember)
        {
            _teamMembers.Add(teamMember);
            return teamMember.Id;
        }

        public void Update(TeamMember teamMember)
        {
            Remove(teamMember.Id);
            Add(teamMember);
        }

        public void Remove(Guid id)
        {
            var teamMember = GetById(id);
            _teamMembers.Remove(teamMember);
        }
    }
}
