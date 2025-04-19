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

        modelBuilder.Entity<Author>().HasData(
            new Author
            {
                Id = 1,
                Name = "Alice",
                Email = "alice@example.com",
                CreatedAt = DateTime.UtcNow,
                Bio = "Tech writer and blogger",
                ProfilePictureUrl = "https://example.com/images/alice.jpg"
            },
            new Author
            {
                Id = 2,
                Name = "Bob",
                Email = "bob@example.com",
                CreatedAt = DateTime.UtcNow,
                Bio = "DevOps expert",
                ProfilePictureUrl = "https://example.com/images/bob.jpg"
            }
        );
    }

    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Author> Authors { get; set; }
}

