namespace Backend.Models;

public class Progress
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public int LessonId { get; set; }
    public bool Completed { get; set; } = false;
    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
}
