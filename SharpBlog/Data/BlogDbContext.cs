using Microsoft.EntityFrameworkCore;
using SharpBlog.Models;

namespace SharpBlog.Data;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasData(
        new User
        {
            Id = 1,
            Name = "Alice",
            Email = "alice@example.com",
            CreatedAt = DateTime.UtcNow,
            Bio = "Tech writer and blogger",
            ProfilePictureUrl = "https://example.com/images/alice.jpg",
            PasswordHash = "placeholder",
            Role = Role.Admin
        },
        new User
        {
            Id = 2,
            Name = "Bob",
            Email = "bob@example.com",
            CreatedAt = DateTime.UtcNow,
            Bio = "DevOps expert",
            ProfilePictureUrl = "https://example.com/images/bob.jpg",
            PasswordHash = "placeholder",
            Role = Role.Author
        },
        new User
        {
            Id = 3,
            Name = "John",
            Email = "john@example.com",
            CreatedAt = DateTime.UtcNow,
            Bio = "Cloud expert",
            ProfilePictureUrl = "https://example.com/images/john.jpg",
            PasswordHash = "placeholder",
            Role = Role.Reader
        }
    );

        // Store roles in User Db as a string instead of enum numbers
        modelBuilder.Entity<User>().Property(i => i.Role).HasConversion<string>();

        // Ensure user emails are unique 
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }

    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<User> Users { get; set; }
}

