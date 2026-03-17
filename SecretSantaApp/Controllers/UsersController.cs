using Microsoft.AspNetCore.Mvc;
using SecretSantaApp.Models;
using SecretSantaApp.Services;

namespace SecretSantaApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = _userRepository.GetAllUsers().ToList();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _userRepository.GetUserByGuid(id);
            return user == null
                ? NotFound("There is no user by this guid!")
                : Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] string name)
        {
            var result = _userRepository.SaveUser(new User(name));
            return result
                ? StatusCode(201, "User created Successfully")
                : Conflict("User with the same name already exists!");
        }

        [HttpPut]
        public IActionResult Put(int id, [FromBody] string name)
        {
            var result = _userRepository.UpdateUser(id, name);
            return result
                ? Ok("User updated Successfully")
                : NotFound("Could not find any user to update!");
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var result = _userRepository.DeleteUser(id);
            return result
                ? Ok("User deleted Successfully")
                : NotFound("Could not find any user to delete!");
        }

        [HttpGet("Match")]
        public IActionResult Match()
        {
            var result = _userRepository.RunMatchingAlgorithm();

            if (result == null)
            {
                return StatusCode(417, "Could Not Match!");
            }

            var data = result.Select(u => new
            {
                Sender = u.Key,
                Receiver = u.Value
            });

            return Ok(data);
        }
    }
}
