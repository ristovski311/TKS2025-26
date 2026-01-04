using Backend.DTOs;
using Backend.Mappings;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly NoteRepository _repository;

        public NotesController(NoteRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll()
        {
            var notes = await _repository.GetAllAsync();
            var noteDtos = notes.Select(n => n.ToDto());
            return Ok(noteDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NoteDto>> GetById(int id)
        {
            var note = await _repository.GetByIdAsync(id);
            if (note == null) return NotFound(new { message = "Note not found" });
            return Ok(note.ToDto());
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<NoteDto>>> GetByCourse(int courseId)
        {
            var notes = await _repository.GetNotesByCourseAsync(courseId);
            var noteDtos = notes.Select(n => n.ToDto());
            return Ok(noteDtos);
        }

        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<NoteDto>>> GetByType(string type)
        {
            var notes = await _repository.GetNotesByTypeAsync(type);
            var noteDtos = notes.Select(n => n.ToDto());
            return Ok(noteDtos);
        }

        [HttpPost]
        public async Task<ActionResult<NoteDto>> Create([FromBody] CreateNoteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var note = dto.ToModel();
            var created = await _repository.CreateAsync(note);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<NoteDto>> Update(int id, [FromBody] UpdateNoteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var note = await _repository.GetByIdAsync(id);
            if (note == null) return NotFound(new { message = "Note not found" });

            note.UpdateFromDto(dto);
            var updated = await _repository.UpdateAsync(note);
            return Ok(updated.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var note = await _repository.GetByIdAsync(id);
            if (note == null) return NotFound(new { message = "Note not found" });

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
