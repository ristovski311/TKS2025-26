using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public bool Completed { get; set; }
        public float? GradeMax { get; set; }
        public float? GradeEarned { get; set; }
        public int CourseId { get; set; }
    }

    public class CreateTaskDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required")]
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        public bool Completed { get; set; } = false;

        [Range(0, 1000, ErrorMessage = "Grade max must be between 0 and 1000")]
        public float? GradeMax { get; set; }

        [Range(0, 1000, ErrorMessage = "Grade earned must be between 0 and 1000")]
        public float? GradeEarned { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }
    }

    public class UpdateTaskDto
    {
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string? Type { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public DateTime? Date { get; set; }

        public bool? Completed { get; set; }

        [Range(0, 1000, ErrorMessage = "Grade max must be between 0 and 1000")]
        public float? GradeMax { get; set; }

        [Range(0, 1000, ErrorMessage = "Grade earned must be between 0 and 1000")]
        public float? GradeEarned { get; set; }

        public int? CourseId { get; set; }
    }
}
