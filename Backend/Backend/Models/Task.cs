using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Backend.Models
{
    [Table("Task")]
    public class TaskItem : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("type")]
        public string Type { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("completed")]
        public bool Completed { get; set; }

        [Column("grade_max")]
        public float? GradeMax { get; set; }

        [Column("grade_earned")]
        public float? GradeEarned { get; set; }

        [Column("course_id")]
        public int CourseId { get; set; }
    }
}
