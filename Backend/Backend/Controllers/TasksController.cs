using Backend.DTOs;
using Backend.Mappings;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskRepository _repository;

        public TasksController(TaskRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetAll()
        {
            var tasks = await _repository.GetAllAsync();
            var taskDtos = tasks.Select(t => t.ToDto());
            return Ok(taskDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetById(int id)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task == null) return NotFound(new { message = "Task not found" });
            return Ok(task.ToDto());
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetByCourse(int courseId)
        {
            var tasks = await _repository.GetTasksByCourseAsync(courseId);
            var taskDtos = tasks.Select(t => t.ToDto());
            return Ok(taskDtos);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetPending()
        {
            var tasks = await _repository.GetPendingTasksAsync();
            var taskDtos = tasks.Select(t => t.ToDto());
            return Ok(taskDtos);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = dto.ToModel();
            var created = await _repository.CreateAsync(task);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> Update(int id, [FromBody] UpdateTaskDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _repository.GetByIdAsync(id);
            if (task == null) return NotFound(new { message = "Task not found" });

            task.UpdateFromDto(dto);
            var updated = await _repository.UpdateAsync(task);
            return Ok(updated.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task == null) return NotFound(new { message = "Task not found" });

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
