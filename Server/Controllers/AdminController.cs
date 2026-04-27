using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<AdminUserListItemDto>>> GetUsersForAdmin(int page = 1)
    {
        const int pageSize = 20;

        var users = await _context.Users
            .OrderByDescending(u => u.IsSuspicious)
            .ThenBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new AdminUserListItemDto
            {
                Id = u.Id,
                Username = u.Username,
                WinRate = u.WinRate,
                IsSuspicious = u.IsSuspicious,
                IsAdmin = u.IsAdmin,
                IsBanned = u.IsBanned
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("make-admin/{id}")]
    public async Task<IActionResult> MakeAdmin(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.IsAdmin = true;
        await _context.SaveChangesAsync();
        return Ok(new { message = $"{user.Username} тепер адмін" });
    }


    [HttpPost("ban/{id}")]
    public async Task<IActionResult> BanUser(int id)
    {
        var targetUser = await _context.Users.FindAsync(id);
        if (targetUser == null) return NotFound();

        if (targetUser.IsAdmin)
        {
            return BadRequest("Не можна забанити адміна без 5 репортів від інших адмінів.");
        }

        targetUser.IsBanned = true;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Користувача забанено" });
    }

    [HttpGet("user-details/{id}")]
    public async Task<IActionResult> GetUserDetails(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return Ok(new {
            user.Id,
            user.Username,
            user.Email,
            user.IsAdmin,
            user.IsSuspicious,
            user.WinRate,
            user.GamesPlayed,
            user.CreatedAt,
            LastSuspiciousMatchId = user.IsSuspicious ? 12345 : (int?)null 
        });
    }
}