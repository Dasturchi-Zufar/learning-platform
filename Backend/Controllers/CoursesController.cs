using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CoursesController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/courses
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? level, [FromQuery] string? technology)
    {
        var query = _db.Courses.AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(c => c.Title.Contains(search) || c.Technology.Contains(search));

        if (!string.IsNullOrEmpty(level))
            query = query.Where(c => c.Level == level);

        if (!string.IsNullOrEmpty(technology))
            query = query.Where(c => c.Technology == technology);

        var courses = await query
            .Select(c => new {
                c.Id, c.Title, c.Description,
                c.Technology, c.Level, c.Rating,
                c.CreatedAt,
                LessonCount = c.Lessons.Count
            })
            .ToListAsync();

        return Ok(courses);
    }

    // GET /api/courses/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _db.Courses
            .Include(c => c.Lessons.OrderBy(l => l.OrderNumber))
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound(new { message = "Kurs topilmadi" });

        return Ok(course);
    }

    // POST /api/courses (Admin only)
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CourseDto dto)
    {
        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            Technology = dto.Technology,
            Level = dto.Level
        };

        _db.Courses.Add(course);
        await _db.SaveChangesAsync();

        return Ok(course);
    }

    // PUT /api/courses/{id} (Admin only)
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CourseDto dto)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null)
            return NotFound(new { message = "Kurs topilmadi" });

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Technology = dto.Technology;
        course.Level = dto.Level;

        await _db.SaveChangesAsync();
        return Ok(course);
    }

    // DELETE /api/courses/{id} (Admin only)
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null)
            return NotFound(new { message = "Kurs topilmadi" });

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Kurs o'chirildi" });
    }
}

public record CourseDto(string Title, string Description, string Technology, string Level);
