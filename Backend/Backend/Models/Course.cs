using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Backend.Models
{
    [Table("Course")]
    public class Course : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("semester")]
        public int Semester { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("grade")]
        public int? Grade { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("professor_id")]
        public int ProfessorId { get; set; }
    }
}
