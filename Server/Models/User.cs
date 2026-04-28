using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    [Table("users")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string Username { get; set; } = string.Empty;
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;
        [Column("games_played")]
        public int GamesPlayed { get; set; }
        [Column("win_rate")]
        public float WinRate { get; set; }
        [Column("rating")]
        public int Rating { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("is_admin")]
        public bool IsAdmin { get; set; }
        [Column("is_suspicious")]
        public bool IsSuspicious { get; set; }
        [Column("is_banned")]
        public bool IsBanned { get; set; }
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }
    }
}