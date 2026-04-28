namespace Server.DTOs;

public class UserProfileDto
{
    public string Username {get; set;} = string.Empty;
    public int GamesPlayed { get; set; }
    public float WinRate { get; set; }
    public int Rating { get; set; }
    public int GlobalRank { get; set; }
    public DateTime CreatedAt { get; set; }
}