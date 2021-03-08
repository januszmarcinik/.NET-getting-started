using System;
using Microsoft.AspNetCore.Mvc;
using NET.GettingStarted.Entities;
using NET.GettingStarted.Services;

namespace NET.GettingStarted.Controllers
{
    [Route("backend")]
    public class BackendController : ControllerBase
    {
        private readonly ITeamMembersService _teamMembersService;

        public BackendController(ITeamMembersService teamMembersService)
        {
            _teamMembersService = teamMembersService;
        }
        
        [HttpGet("")]
        public IActionResult Get()
        {
            //throw new NotImplementedException("Not implemented in BackendController");
            var backend = _teamMembersService.GetByRoles(Role.Java, Role.DotNet);
            return Ok(backend);
        }
    }
}
