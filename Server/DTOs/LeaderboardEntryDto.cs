namespace Server.DTOs;

public class LeaderboardEntryDto
{
    public string Username {get; set;} = String.Empty;
    public int Rating {get; set;}
}

public class LeaderboardResponseDto
{
    public List<LeaderboardEntryDto> TopPlayers { get; set; } = new();
    public int TotalPlayers { get; set; }
    public int CurrentPage { get; set; }
    public int? UserRank { get; set; }
}