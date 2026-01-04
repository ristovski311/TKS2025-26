using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Backend.Models
{
    [Table("Note")]
    public class Note : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("content")]
        public string? Content { get; set; }

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; }

        [Column("type")]
        public string Type { get; set; } = string.Empty;

        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("parent_id")]
        public int? ParentId { get; set; }
    }
}
