using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.DTOs;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<UserProfileDto>> GetProfile(int id) {

        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "Користувача не знайдено" });
        }

        var rank = await _context.Users.CountAsync(u => u.Rating > user.Rating) + 1;

        var profile = new UserProfileDto
        {
            Username = user.Username,
            GamesPlayed = user.GamesPlayed,
            WinRate = user.WinRate,
            Rating = user.Rating,
            GlobalRank = rank,
            CreatedAt = user.CreatedAt
        };

        return Ok(profile);
    }
}