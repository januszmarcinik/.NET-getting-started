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
        public IActionResult GetById(Guid id) =>
            Ok(_service.GetById(id));

        [HttpPost]
        public IActionResult Post([FromBody] TeamMember teamMember)
        {
            var id = _service.Add(teamMember);
            return Accepted($"Successfully created team member with id '{id}'.");
        }

        [HttpPut]
        public IActionResult Put([FromBody] TeamMember teamMember)
        {
            _service.Update(teamMember);
            return Accepted("Successfully updated team member.");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _service.Remove(id);
            return Accepted("Successfully deleted team member.");
        }

        public override AcceptedResult Accepted(string message)
        {
            _logger.LogInformation(message);
            return Accepted(value: message);
        }
    }
}
