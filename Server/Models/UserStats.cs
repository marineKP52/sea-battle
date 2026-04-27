using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Server.Models;

[Table("users_stats")]
public class UserStats
{
    [Key]
    [Column("user_id")]
    public int Id { get; set; }
    [Column("total_matches")]
    public int TotalMatches {get; set;}
    [Column("total_hits")]
    public int TotalHits {get; set;}
    [Column("total_shots")]
    public int TotalShots {get; set;}
}