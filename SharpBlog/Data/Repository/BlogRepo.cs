using Microsoft.EntityFrameworkCore;
using SharpBlog.Models;
using SharpBlog.Models.DTOs;
using System.Globalization;

namespace SharpBlog.Data.Repository;

public class BlogRepo : IBlogRepo
{
    private readonly BlogDbContext _context;

    public BlogRepo(BlogDbContext context)
    {
        _context = context;
    }


    public async Task<PagedResult<BlogPostResponseDTO>> GetAllPosts(
        string? author, string? tag, string? category, string? search,
        string? sortBy, bool isDescending, int pageNumber, int pageSize)
    {
        var query = _context.BlogPosts
            .AsNoTracking()
            .Include(bp => bp.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(author))
            query = query.Where(bp => bp.User.Name == author);

        if (!string.IsNullOrEmpty(tag))
            query = query.Where(bp => bp.Tags == tag);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(bp => bp.Category == category);

        // Full-text search
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(bp =>
                EF.Functions.Like(bp.Title, $"%{search}%") ||
                EF.Functions.Like(bp.Content, $"%{search}%") ||
                EF.Functions.Like(bp.Category, $"%{search}%"));
        }

        // Sorting
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "title" => isDescending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                "createdat" => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                "author" => isDescending ? query.OrderByDescending(p => p.User.Name) : query.OrderBy(p => p.User.Name),
                _ => query.OrderByDescending(p => p.CreatedAt) // default fallback
            };
        }
        else
        {
            query = query.OrderByDescending(p => p.CreatedAt); // default sort
        }

        // pagination
        var totalItems = await query.CountAsync();

        int skipAmount = pageSize * (pageNumber - 1);

        var posts = await query
            .Skip(skipAmount)
            .Take(pageSize)
            .ToListAsync();

        var postDTOs = posts.Select(bp => new BlogPostResponseDTO
        {
            Id = bp.Id,
            Title = bp.Title,
            Content = bp.Content,
            AuthorName = bp.User.Name,
            Tags = bp.Tags,
            Category = bp.Category,
            CreatedAt = bp.CreatedAt,
            UpdatedAt = bp.UpdatedAt
        });

        return new PagedResult<BlogPostResponseDTO>
        {
            Items = postDTOs,
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<BlogPostResponseDTO> GetPost(int id)
    {
        var blogPost = await _context.BlogPosts
            .AsNoTracking()
            .Include(bp => bp.User)
            .FirstOrDefaultAsync(bp => bp.Id == id);

        if (blogPost == null) return null;

        return new BlogPostResponseDTO
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Content = blogPost.Content,
            AuthorName = blogPost.User.Name,
            Tags = blogPost.Tags,
            Category = blogPost.Category,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt
        };
    }

    public async Task<BlogPostResponseDTO> CreatePost(BlogPostDTO blogPostDto, int userId)
    {
        var author = await _context.Users.FindAsync(userId);
        if (author == null)
        {
            throw new ArgumentException($"User with ID {userId} not found.");
        }

        var newBlogPost = new BlogPost
        {
            Title = blogPostDto.Title,
            Content = blogPostDto.Content,
            User = author,
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

    // TODO: Refactor to make use of exceptions instead of bool
    public async Task<bool> UpdatePost(int postId, BlogPostDTO blogPostDto, int userId)
    {
        var updateBlogPost = await _context.BlogPosts.FindAsync(postId);

        if (updateBlogPost == null)
            throw new ArgumentException($"Blog post with ID {postId} not found.");

        // Optional: Admins can update any post; authors only their own
        if (updateBlogPost.UserId != userId)
            //throw new ArgumentException($"Blog post with ID {postId} not found.");
            return false; // Forbidden

        updateBlogPost.Title = blogPostDto.Title;
        updateBlogPost.Content = blogPostDto.Content;
        updateBlogPost.Category = blogPostDto.Category;
        updateBlogPost.Tags = blogPostDto.Tags;
        updateBlogPost.UpdatedAt = DateTime.UtcNow;

        _context.BlogPosts.Update(updateBlogPost);
        await _context.SaveChangesAsync();

        return true;
    }

    // TODO: Refactor to make use of exceptions instead of bool
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

    public async Task<IEnumerable<BlogPostResponseDTO>> GetAuthorPosts(int authorId)
    {
        var blogPosts = await _context.BlogPosts
            .Include(bp => bp.User)
            .Where(bp => bp.UserId == authorId)
            .ToListAsync();

        return blogPosts.Select(blogPost => new BlogPostResponseDTO
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Content = blogPost.Content,
            AuthorName = blogPost.User.Name,
            Tags = blogPost.Tags,
            Category = blogPost.Category,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt
        });
    }

    public async Task<BlogPostResponseDTO> GetAuthorPost(int userId, int blogId)
    {
        var blogPost = await _context.BlogPosts
            .Include(bp => bp.User)
            .FirstOrDefaultAsync(bp => bp.Id == blogId && bp.UserId == userId);

        if (blogPost == null) return null;

        return new BlogPostResponseDTO
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Content = blogPost.Content,
            AuthorName = blogPost.User.Name,
            Tags = blogPost.Tags,
            Category = blogPost.Category,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt
        };
    }

}
