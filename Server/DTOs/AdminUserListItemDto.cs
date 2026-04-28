namespace Server.DTOs;



public class AdminUserListItemDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int GamesPlayed { get; set; }
    public float WinRate { get; set; }
    public int TotalHits {get; set;}
    public int TotalShots {get; set;}
    public bool IsSuspicious { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsBanned { get; set; }
}