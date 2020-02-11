using Microsoft.AspNetCore.Mvc;
using NETCore3.Entities;
using NETCore3.Services;

namespace NETCore3.Controllers
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
            var backend = _teamMembersService.GetByRoles(Role.Java, Role.DotNet);
            return Ok(backend);
        }
    }
}
