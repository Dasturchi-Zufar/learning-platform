using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api")]
public class LessonsController : ControllerBase
{
    private readonly AppDbContext _db;

    public LessonsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/courses/{courseId}/lessons
    [HttpGet("courses/{courseId}/lessons")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var lessons = await _db.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.OrderNumber)
            .ToListAsync();

        return Ok(lessons);
    }

    // GET /api/lessons/{id}
    [HttpGet("lessons/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var lesson = await _db.Lessons.FindAsync(id);
        if (lesson == null)
            return NotFound(new { message = "Dars topilmadi" });

        return Ok(lesson);
    }

    // POST /api/lessons (Admin only)
    [HttpPost("lessons")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] LessonDto dto)
    {
        var lesson = new Lesson
        {
            CourseId = dto.CourseId,
            Title = dto.Title,
            Content = dto.Content,
            VideoUrl = dto.VideoUrl,
            OrderNumber = dto.OrderNumber
        };

        _db.Lessons.Add(lesson);
        await _db.SaveChangesAsync();
        return Ok(lesson);
    }

    // PUT /api/lessons/{id} (Admin only)
    [HttpPut("lessons/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] LessonDto dto)
    {
        var lesson = await _db.Lessons.FindAsync(id);
        if (lesson == null)
            return NotFound(new { message = "Dars topilmadi" });

        lesson.Title = dto.Title;
        lesson.Content = dto.Content;
        lesson.VideoUrl = dto.VideoUrl;
        lesson.OrderNumber = dto.OrderNumber;

        await _db.SaveChangesAsync();
        return Ok(lesson);
    }

    // DELETE /api/lessons/{id} (Admin only)
    [HttpDelete("lessons/{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var lesson = await _db.Lessons.FindAsync(id);
        if (lesson == null)
            return NotFound(new { message = "Dars topilmadi" });

        _db.Lessons.Remove(lesson);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Dars o'chirildi" });
    }
}

public record LessonDto(int CourseId, string Title, string Content, string? VideoUrl, int OrderNumber);
