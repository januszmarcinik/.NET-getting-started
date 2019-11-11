using System;
using System.Collections.Generic;
using NETCore3.Entities;

namespace NETCore3.Services
{
    internal interface ITeamMembersService
    {
        TeamMember GetById(Guid id);
        
        IEnumerable<TeamMember> GetAll();
        
        Guid Add(TeamMember teamMember);
        
        void Update(TeamMember teamMember);
        
        void Remove(Guid id);
    }
}