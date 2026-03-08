using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/courses")]
public class RatingsController : ControllerBase
{
    private readonly AppDbContext _db;

    public RatingsController(AppDbContext db)
    {
        _db = db;
    }

    // POST /api/courses/{id}/rate
    [HttpPost("{id}/rate")]
    [Authorize]
    public async Task<IActionResult> Rate(int id, [FromBody] RateDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var existing = await _db.Ratings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == id);

        if (existing != null)
            return BadRequest(new { message = "Siz allaqachon baho bergansiz" });

        var rating = new Rating
        {
            UserId = userId,
            CourseId = id,
            RatingValue = dto.Rating
        };

        _db.Ratings.Add(rating);

        // Kurs o'rtacha ratingini yangilash
        var course = await _db.Courses.FindAsync(id);
        if (course != null)
        {
            var allRatings = await _db.Ratings
                .Where(r => r.CourseId == id)
                .ToListAsync();
            course.Rating = (decimal)(allRatings.Sum(r => r.RatingValue) + dto.Rating) / (allRatings.Count + 1);
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Baho berildi" });
    }
}

public record RateDto(int Rating);
