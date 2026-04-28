namespace Server.DTOs;

public class LeaderboardEntryDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Rating { get; set; }
    public int Position { get; set; }
}
