using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Progress> Progresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Course>().ToTable("Courses");
        modelBuilder.Entity<Lesson>().ToTable("Lessons");
        modelBuilder.Entity<Progress>().ToTable("Progress");

        modelBuilder.Entity<Rating>().ToTable("Ratings")
            .HasIndex(r => new { r.UserId, r.CourseId })
            .IsUnique();

        modelBuilder.Entity<Rating>()
            .Property(r => r.RatingValue)
            .HasColumnName("Rating");
    }
}
