using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NETCore3.Entities;
using NETCore3.Services;

namespace NETCore3.Controllers
{
    [Route("api/team-members")]
    public class TeamMembersController : ControllerBase
    {
        private readonly ITeamMembersService _service;
        private readonly ILogger _logger;

        public TeamMembersController(ITeamMembersService service, ILogger<TeamMembersController> logger)
        {
            _service = service;
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult GetAll() =>
            Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var teamMember = _service.GetById(id);
            if (teamMember == null)
            {
                _logger.LogError("Team member with given id {ID} does not exist", id);
                return NotFound($"Team member with given id '{id}' does not exist.");
            }

            return Ok(teamMember);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TeamMember teamMember)
        {
            var id = _service.Add(teamMember);
            
            _logger.LogInformation("Successfully created team member with id {ID}", id);
            return Accepted($"Successfully created team member with id '{id}'.");
        }

        [HttpPut]
        public IActionResult Put([FromBody] TeamMember teamMember)
        {
            _service.Update(teamMember);
            
            const string message = "Successfully updated team member."; 
            _logger.LogInformation(message);
            return Ok(message);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var teamMember = _service.GetById(id);
            if (teamMember == null)
            {
                _logger.LogError("Team member with given id {ID} does not exist", id);
                return NotFound($"Team member with given id '{id}' does not exist.");
            }
            
            _service.Remove(teamMember);

            const string message = "Successfully deleted team member.";
            _logger.LogInformation(message);
            return Ok(message);
        }
    }
}
