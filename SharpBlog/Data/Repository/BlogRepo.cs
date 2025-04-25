using Microsoft.EntityFrameworkCore;
using SharpBlog.Models;
using SharpBlog.Models.DTOs;

namespace SharpBlog.Data.Repository;

public class BlogRepo : IBlogRepo
{
    private readonly BlogDbContext _context;

    public BlogRepo(BlogDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<BlogPostResponseDTO>> GetAllPosts(string? author, string? tag, string? category)
    {
        var query = _context.BlogPosts
                           .Include(bp => bp.Author)
                           .AsQueryable();

        if (!string.IsNullOrEmpty(author))
            query = query.Where(bp => bp.Author.Name == author);

        if (!string.IsNullOrEmpty(tag))
            query = query.Where(bp => bp.Tags == tag); 
        
        if (!string.IsNullOrEmpty(category))
            query = query.Where(bp => bp.Category == category);

        var data = await query.ToListAsync();

        return data.Select(blogPost => new BlogPostResponseDTO
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Content = blogPost.Content,
            AuthorName = blogPost.Author.Name,
            Tags = blogPost.Tags,
            Category = blogPost.Category,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt
        });
    }

    public async Task<BlogPostResponseDTO> GetPost(int id)
    {
        var blogPost = await _context.BlogPosts
        .Include(bp => bp.Author)
        .FirstOrDefaultAsync(bp => bp.Id == id);

        if (blogPost == null) return null;

        return new BlogPostResponseDTO
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Content = blogPost.Content,
            AuthorName = blogPost.Author.Name,
            Tags = blogPost.Tags,
            Category = blogPost.Category
        };
    }

    public async Task<BlogPostResponseDTO> CreatePost(BlogPostDTO blogPostDto)
    {
        var author = await _context.Authors.FindAsync(blogPostDto.AuthorId);
        if (author == null)
        {
            throw new ArgumentException($"Author with ID {blogPostDto.AuthorId} not found.");
        }


        var newBlogPost = new BlogPost
        {
            Title = blogPostDto.Title,
            Content = blogPostDto.Content,
            Author = author,
            Category = blogPostDto.Category,
            Tags = blogPostDto.Tags,
            CreatedAt = DateTime.UtcNow
        };

        await _context.BlogPosts.AddAsync(newBlogPost);
        await _context.SaveChangesAsync();

        return new BlogPostResponseDTO
        {
            Id = newBlogPost.Id,
            Title = newBlogPost.Title,
            Content = newBlogPost.Content,
            AuthorName = author.Name,
            Tags = newBlogPost.Tags,
            Category = newBlogPost.Category,
            CreatedAt = newBlogPost.CreatedAt
        };
    }

    public async Task UpdatePost(int id, BlogPostDTO blogPostDto)
    {
        var updateBlogPost = await _context.BlogPosts.FindAsync(id);
        var author = await _context.Authors.FindAsync(blogPostDto.AuthorId);

        if (id != updateBlogPost.Id)
        {
            throw new ArgumentException($"Blog post with ID {id} not found.");

        }
        if (updateBlogPost == null)
        {
            throw new ArgumentException($"Blog post with ID {id} not found.");
        }


        if (author != null)
        {
            updateBlogPost.Title = blogPostDto.Title;
            updateBlogPost.Content = blogPostDto.Content;
            updateBlogPost.Category = blogPostDto.Category;
            updateBlogPost.Tags = blogPostDto.Tags;
            updateBlogPost.UpdatedAt = DateTime.UtcNow;

            _context.BlogPosts.Update(updateBlogPost);

            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> DeletePost(int id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        if (blogPost == null)
        {
            return false;
        }

        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();

        return true;
    }
}
