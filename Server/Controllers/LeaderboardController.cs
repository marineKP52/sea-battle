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
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetLeaderboard(int page = 1, int userId = 0)
    {
        const int pageSize = 10;
        if (page < 1) page = 1;

        var userEntities = await _context.Users
            .OrderByDescending(u => u.Rating)
            .ThenBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var players = userEntities.Select((u, index) => new LeaderboardEntryDto
        {
            Id = u.Id,
            Username = u.Username,
            Rating = u.Rating,
            Position = ((page - 1) * pageSize) + index + 1
        }).ToList();

        var currentUser = await _context.Users.FindAsync(userId);
        
        if (currentUser != null)
        {
            bool isUserOnPage = players.Any(p => p.Id == userId);

            if (isUserOnPage)
            {
                var lastUser = await _context.Users
                    .OrderBy(u => u.Rating)
                    .ThenByDescending(u => u.Id)
                    .FirstOrDefaultAsync();

                if (lastUser != null && !players.Any(p => p.Id == lastUser.Id))
                {
                    var totalCount = await _context.Users.CountAsync();
                    players.Add(new LeaderboardEntryDto
                    {
                        Id = lastUser.Id,
                        Username = lastUser.Username,
                        Rating = lastUser.Rating,
                        Position = totalCount
                    });
                }
            }
            else
            {
                var userRank = await _context.Users.CountAsync(u => u.Rating > currentUser.Rating) + 1;
                
                players.Add(new LeaderboardEntryDto
                {
                    Id = currentUser.Id,
                    Username = currentUser.Username,
                    Rating = currentUser.Rating,
                    Position = userRank
                });
            }
        }

        return Ok(players);
    }
}