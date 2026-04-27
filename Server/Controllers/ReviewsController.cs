using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.DTOs;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> LeaveReview(int userId, [FromBody] CreateReviewDto dto)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
        {
            return BadRequest("Оцінка повинна бути від 1 до 5.");
        }

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) return NotFound("Користувача не знайдено.");

        var review = new Review
        {
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Відгук успішно залишено!" });
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews()
    {
        var reviews = await _context.Reviews
            .Join(
                _context.Users,           
                review => review.UserId, 
                user => user.Id,          
                (review, user) => new 
                {
                    Username = user.Username,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt
                }
            )
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(reviews);
    }
}