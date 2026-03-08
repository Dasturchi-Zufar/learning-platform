namespace Backend.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Technology { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public decimal Rating { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Lesson> Lessons { get; set; } = new();
}
