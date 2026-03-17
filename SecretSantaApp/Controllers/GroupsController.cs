using Microsoft.AspNetCore.Mvc;
using SecretSantaApp.Models;
using SecretSantaApp.Services;

namespace SecretSantaApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository _groupService;

        public GroupsController(IGroupRepository groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var groups = _groupService.GetAllGroups().ToList();
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var group = _groupService.GetGroupById(id);
            return group == null
                ? NotFound("There is no group by this guid!")
                : Ok(group);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Group group)
        {
            var result = _groupService.SaveGroup(group);
            return result
                ? StatusCode(201, "Group created Successfully")
                : Conflict("Group with the same name already exists!");
        }

        [HttpPut]
        public IActionResult Put(int id, [FromBody] List<User> users)
        {
            var result = _groupService.UpdateGroup(id, users);
            return result
                ? Ok("Group updated Successfully")
                : NotFound("Could not find any group to update!");
        }

        [HttpDelete]
        public IActionResult Delete(int id, [FromBody] User user)
        {
            var result = _groupService.RemoveUserFromGroup(id, user);
            return result
                ? Ok("Group deleted Successfully")
                : NotFound("Could not find any group to delete!");
        }
    }
}
