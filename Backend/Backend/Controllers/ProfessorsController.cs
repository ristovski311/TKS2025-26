using Backend.DTOs;
using Backend.Mappings;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorsController : ControllerBase
    {
        private readonly ProfessorRepository _repository;

        public ProfessorsController(ProfessorRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfessorDto>>> GetAll()
        {
            var professors = await _repository.GetAllAsync();
            var professorDtos = professors.Select(p => p.ToDto());
            return Ok(professorDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfessorDto>> GetById(int id)
        {
            var professor = await _repository.GetByIdAsync(id);
            if (professor == null) return NotFound(new { message = "Professor not found" });
            return Ok(professor.ToDto());
        }

        [HttpPost]
        public async Task<ActionResult<ProfessorDto>> Create([FromBody] CreateProfessorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var professor = dto.ToModel();
            var created = await _repository.CreateAsync(professor);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProfessorDto>> Update(int id, [FromBody] UpdateProfessorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var professor = await _repository.GetByIdAsync(id);
            if (professor == null) return NotFound(new { message = "Professor not found" });

            professor.UpdateFromDto(dto);
            var updated = await _repository.UpdateAsync(professor);
            return Ok(updated.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var professor = await _repository.GetByIdAsync(id);
            if (professor == null) return NotFound(new { message = "Professor not found" });

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly CourseRepository _repository;

        public CoursesController(CourseRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
        {
            var courses = await _repository.GetAllAsync();
            var courseDtos = courses.Select(c => c.ToDto());
            return Ok(courseDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetById(int id)
        {
            var course = await _repository.GetByIdAsync(id);
            if (course == null) return NotFound(new { message = "Course not found" });
            return Ok(course.ToDto());
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetByUser(int userId)
        {
            var courses = await _repository.GetCoursesByUserAsync(userId);
            var courseDtos = courses.Select(c => c.ToDto());
            return Ok(courseDtos);
        }

        [HttpGet("professor/{professorId}")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetByProfessor(int professorId)
        {
            var courses = await _repository.GetCoursesByProfessorAsync(professorId);
            var courseDtos = courses.Select(c => c.ToDto());
            return Ok(courseDtos);
        }

        [HttpPost]
        public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = dto.ToModel();
            var created = await _repository.CreateAsync(course);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> Update(int id, [FromBody] UpdateCourseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _repository.GetByIdAsync(id);
            if (course == null) return NotFound(new { message = "Course not found" });

            course.UpdateFromDto(dto);
            var updated = await _repository.UpdateAsync(course);
            return Ok(updated.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var course = await _repository.GetByIdAsync(id);
            if (course == null) return NotFound(new { message = "Course not found" });

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
