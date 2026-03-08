namespace Backend.Models;

public class Rating
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public int RatingValue { get; set; }
    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
