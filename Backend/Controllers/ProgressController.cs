using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/progress")]
public class ProgressController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProgressController(AppDbContext db)
    {
        _db = db;
    }

    // POST /api/progress/complete
    [HttpPost("complete")]
    [Authorize]
    public async Task<IActionResult> Complete([FromBody] ProgressDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var existing = await _db.Progresses
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == dto.LessonId);

        if (existing != null)
        {
            existing.Completed = true;
        }
        else
        {
            _db.Progresses.Add(new Progress
            {
                UserId = userId,
                CourseId = dto.CourseId,
                LessonId = dto.LessonId,
                Completed = true
            });
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Dars tugallandi" });
    }

    // GET /api/progress/user
    [HttpGet("user")]
    [Authorize]
    public async Task<IActionResult> GetUserProgress()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var progress = await _db.Progresses
            .Where(p => p.UserId == userId)
            .Include(p => p.Course)
            .Include(p => p.Lesson)
            .ToListAsync();

        var grouped = progress
            .GroupBy(p => p.Course)
            .Select(g => new {
                Course = new { g.Key.Id, g.Key.Title, g.Key.Technology },
                CompletedLessons = g.Count(p => p.Completed),
                TotalLessons = g.Key.Lessons.Count
            });

        return Ok(grouped);
    }
}

public record ProgressDto(int CourseId, int LessonId);
