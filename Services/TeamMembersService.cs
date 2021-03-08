using System;
using System.Collections.Generic;
using System.Linq;
using NET.GettingStarted.Entities;

namespace NET.GettingStarted.Services
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

        public IEnumerable<TeamMember> GetByRoles(params Role[] roles) =>
            _teamMembers
                .Where(x => roles.Contains(x.Role))
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

        public static TeamMembersService CreateDefault()
        {
            var initTeamMembers = new[]
            {
                new TeamMember(Guid.NewGuid(), "John", Role.DotNet, 5),
                new TeamMember(Guid.NewGuid(), "Franc", Role.DotNet, 6),
                new TeamMember(Guid.NewGuid(), "Robert", Role.Java, 2),
                new TeamMember(Guid.NewGuid(), "Alex", Role.Angular, 5),
                new TeamMember(Guid.NewGuid(), "Jack", Role.React, 3),
                new TeamMember(Guid.NewGuid(), "Tom", Role.Angular, 6)
            };
            
            return new TeamMembersService(initTeamMembers);
        }
        
        public static TeamMembersService CreateEmpty() => new TeamMembersService();
    }
}
