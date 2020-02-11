using Microsoft.AspNetCore.Mvc;
using NETCore3.Entities;
using NETCore3.Services;

namespace NETCore3.Controllers
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
            var frontend = _teamMembersService.GetByRoles(Role.Angular, Role.React); 
            return Ok(frontend);
        }
    }
}
