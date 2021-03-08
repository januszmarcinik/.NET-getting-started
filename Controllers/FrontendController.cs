using System;
using Microsoft.AspNetCore.Mvc;
using NET.GettingStarted.Entities;
using NET.GettingStarted.Services;

namespace NET.GettingStarted.Controllers
{
    [Route("frontend")]
    public class FrontendController : ControllerBase
    {
        private readonly ITeamMembersService _teamMembersService;

        public FrontendController(ITeamMembersService teamMembersService)
        {
            _teamMembersService = teamMembersService;
        }
        
        [HttpGet("")]
        public IActionResult Get()
        {
            //throw new NotImplementedException("Not implemented in FrontendController");
            var frontend = _teamMembersService.GetByRoles(Role.Angular, Role.React); 
            return Ok(frontend);
        }
    }
}
