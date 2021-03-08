using System;
using System.Collections.Generic;
using NET.GettingStarted.Entities;

namespace NET.GettingStarted.Services
{
    public interface ITeamMembersService
    {
        TeamMember GetById(Guid id);
        
        IEnumerable<TeamMember> GetAll();
        
        IEnumerable<TeamMember> GetByRoles(params Role[] roles);
        
        Guid Add(TeamMember teamMember);
        
        void Update(TeamMember teamMember);
        
        void Remove(TeamMember teamMember);
    }
}