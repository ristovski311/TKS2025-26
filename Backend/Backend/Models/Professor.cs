using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Backend.Models
{
    [Table("Professor")]
    public class Professor : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Column("mail")]
        public string? Mail { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("office")]
        public string? Office { get; set; }
    }
}
