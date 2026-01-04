using Backend.DTOs;
using Backend.Mappings;
using Backend.Models;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _repository;

        public UsersController(UserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _repository.GetAllAsync();
            var userDtos = users.Select(u => u.ToDto());
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });
            return Ok(user.ToDto());
        }

        [HttpGet("semester/{semester}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetBySemester(int semester)
        {
            var users = await _repository.GetUsersBySemesterAsync(semester);
            var userDtos = users.Select(u => u.ToDto());
            return Ok(userDtos);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = dto.ToModel();
            var created = await _repository.CreateAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _repository.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            user.UpdateFromDto(dto);
            var updated = await _repository.UpdateAsync(user);
            return Ok(updated.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
