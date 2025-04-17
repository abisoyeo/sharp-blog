using Microsoft.EntityFrameworkCore;

namespace SharpBlog.Models;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    public DbSet<BlogPost> BlogPosts { get; set; }
    //public DbSet<Author> Authors { get; set; }
}

