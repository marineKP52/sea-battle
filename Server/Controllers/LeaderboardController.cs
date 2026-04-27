using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public LeaderboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<LeaderboardResponseDto>> GetLeaderboard(int page = 1, int? userId = null)
    {
        int pageSize = 10;

        var players = await _context.Users.OrderByDescending(u => u.Rating)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(u => new LeaderboardEntryDto
        {
            Username = u.Username,
            Rating = u.Rating
        })
        .ToListAsync();

        var totalPlayers = await _context.Users.CountAsync();

        int? rank = null;

        if (userId.HasValue)
        {
            var user = await _context.Users.FindAsync(userId.Value);
            if (user != null)
            {
                rank = await _context.Users.CountAsync(u => u.Rating > user.Rating) + 1;
            }
        }

        return Ok(new LeaderboardResponseDto
        {
            TopPlayers = players,
            TotalPlayers = totalPlayers,
            CurrentPage = page,
            UserRank = rank
        });
    }
}