using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class NoteDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Content { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Type { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int? ParentId { get; set; }
    }

    public class CreateNoteDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public string? Content { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }

        public int? ParentId { get; set; }
    }

    public class UpdateNoteDto
    {
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public string? Content { get; set; }

        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string? Type { get; set; }

        public int? CourseId { get; set; }

        public int? ParentId { get; set; }
    }
}
