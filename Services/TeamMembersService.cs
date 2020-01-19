using System;
using System.Collections.Generic;
using System.Linq;
using NETCore3.Entities;

namespace NETCore3.Services
{
    internal class TeamMembersService : ITeamMembersService
    {
        private readonly HashSet<TeamMember> _teamMembers;

        public TeamMembersService(params TeamMember[] initTeamMembers)
        {
            _teamMembers = new HashSet<TeamMember>(initTeamMembers);
        }

        public TeamMember GetById(Guid id) => 
            _teamMembers.SingleOrDefault(x => x.Id == id);

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
            Remove(teamMember);
            Add(teamMember);
        }

        public void Remove(TeamMember teamMember) => 
            _teamMembers.Remove(teamMember);
    }
}
