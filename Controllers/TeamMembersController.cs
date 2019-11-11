using System;
using Microsoft.AspNetCore.Mvc;
using NETCore3.Entities;
using NETCore3.Services;

namespace NETCore3.Controllers
{
    [Route("api/team-members")]
    public class TeamMembersController : ControllerBase
    {
        private readonly ITeamMembersService _service;

        public TeamMembersController(ITeamMembersService service)
        {
            _service = service;
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
            return Accepted(value: $"Successfully created team member with id '{id}'.");
        }

        [HttpPut]
        public IActionResult Put([FromBody] TeamMember teamMember)
        {
            _service.Update(teamMember);
            return Accepted(value: "Successfully updated team member.");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _service.Remove(id);
            return Accepted(value: "Successfully deleted team member.");
        }
    }
}
