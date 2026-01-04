using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string? Description { get; set; }
        public int? Grade { get; set; }
        public int UserId { get; set; }
        public int ProfessorId { get; set; }
    }
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Range(1, 12, ErrorMessage = "Semester must be between 1 and 12")]
        public int Semester { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(1, 100, ErrorMessage = "Grade must be between 1 and 100")]
        public int? Grade { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Professor ID is required")]
        public int ProfessorId { get; set; }
    }

    public class UpdateCourseDto
    {
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [Range(1, 12, ErrorMessage = "Semester must be between 1 and 12")]
        public int? Semester { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(1, 100, ErrorMessage = "Grade must be between 1 and 100")]
        public int? Grade { get; set; }

        public int? UserId { get; set; }

        public int? ProfessorId { get; set; }
    }
}
