using Microsoft.EntityFrameworkCore;
using SharpBlog.Data.Repository;
using SharpBlog.Data;
using SharpBlog.Models.DTOs;
using SharpBlog.Models;

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
            .Include(bp => bp.Tags)
            .AsQueryable();

        if (!string.IsNullOrEmpty(author)) 
            query = query.Where(bp => bp.User.Name == author);

        if (!string.IsNullOrEmpty(tag))
            query = query.Where(bp => bp.Tags.Any(t => t.Name == tag));

        if (!string.IsNullOrEmpty(category))
            query = query.Where(bp => bp.Category == category);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(bp =>
                EF.Functions.Like(bp.Title, $"%{search}%") ||
                EF.Functions.Like(bp.Content, $"%{search}%") ||
                EF.Functions.Like(bp.Category, $"%{search}%"));
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "title" => isDescending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                "createdat" => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                "author" => isDescending ? query.OrderByDescending(p => p.User.Name) : query.OrderBy(p => p.User.Name),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(p => p.CreatedAt);
        }

        var totalItems = await query.CountAsync();
        var skipAmount = pageSize * (pageNumber - 1);

        var posts = await query.Skip(skipAmount).Take(pageSize).ToListAsync();

        var postDTOs = posts.Select(bp => new BlogPostResponseDTO
        {
            Id = bp.Id,
            Title = bp.Title,
            Content = bp.Content,
            AuthorName = bp.User.Name,
            Tags = bp.Tags.Select(t => t.Name).ToList(),
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
            .Include(bp => bp.Tags)
            .FirstOrDefaultAsync(bp => bp.Id == id);

        if (blogPost == null) return null;

        return new BlogPostResponseDTO
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Content = blogPost.Content,
            AuthorName = blogPost.User.Name,
            Tags = blogPost.Tags.Select(t => t.Name).ToList(),
            Category = blogPost.Category,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt
        };
    }

    public async Task<BlogPostResponseDTO> CreatePost(BlogPostDTO blogPostDto, int userId)
    {
        var author = await _context.Users.FindAsync(userId);
        if (author == null)
            throw new ArgumentException($"User with ID {userId} not found.");

        var tagEntities = new List<Tag>();
        foreach (var tagName in blogPostDto.Tags.Distinct())
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                _context.Tags.Add(tag);
            }
            tagEntities.Add(tag);
        }

        await _context.SaveChangesAsync();

        var newBlogPost = new BlogPost
        {
            Title = blogPostDto.Title,
            Content = blogPostDto.Content,
            User = author,
            Category = blogPostDto.Category,
            Tags = tagEntities,
            CreatedAt = DateTime.UtcNow
        };

        _context.BlogPosts.Add(newBlogPost);
        await _context.SaveChangesAsync();

        return new BlogPostResponseDTO
        {
            Id = newBlogPost.Id,
            Title = newBlogPost.Title,
            Content = newBlogPost.Content,
            AuthorName = author.Name,
            Tags = tagEntities.Select(t => t.Name).ToList(),
            Category = newBlogPost.Category,
            CreatedAt = newBlogPost.CreatedAt
        };
    }

    public async Task<bool> UpdatePost(int postId, BlogPostDTO blogPostDto, int userId)
    {
        var blogPost = await _context.BlogPosts
            .Include(bp => bp.Tags)
            .FirstOrDefaultAsync(bp => bp.Id == postId);

        if (blogPost == null)
            throw new ArgumentException($"Blog post with ID {postId} not found.");

        if (blogPost.UserId != userId)
            return false;

        blogPost.Title = blogPostDto.Title;
        blogPost.Content = blogPostDto.Content;
        blogPost.Category = blogPostDto.Category;
        blogPost.UpdatedAt = DateTime.UtcNow;

        // Manage tags
        blogPost.Tags.Clear();
        foreach (var tagName in blogPostDto.Tags.Distinct())
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }
            blogPost.Tags.Add(tag);
        }

        _context.BlogPosts.Update(blogPost);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeletePost(int id)
    {
        var blogPost = await _context.BlogPosts.FindAsync(id);
        if (blogPost == null)
            return false;

        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<BlogPostResponseDTO>> GetAuthorPosts(int authorId)
    {
        var blogPosts = await _context.BlogPosts
            .Include(bp => bp.User)
            .Include(bp => bp.Tags)
            .Where(bp => bp.UserId == authorId)
            .ToListAsync();

        return blogPosts.Select(blogPost => new BlogPostResponseDTO
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Content = blogPost.Content,
            AuthorName = blogPost.User.Name,
            Tags = blogPost.Tags.Select(t => t.Name).ToList(),
            Category = blogPost.Category,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt
        });
    }

    public async Task<BlogPostResponseDTO> GetAuthorPost(int userId, int blogId)
    {
        var blogPost = await _context.BlogPosts
            .Include(bp => bp.User)
            .Include(bp => bp.Tags)
            .FirstOrDefaultAsync(bp => bp.Id == blogId && bp.UserId == userId);

        if (blogPost == null) return null;

        return new BlogPostResponseDTO
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Content = blogPost.Content,
            AuthorName = blogPost.User.Name,
            Tags = blogPost.Tags.Select(t => t.Name).ToList(),
            Category = blogPost.Category,
            CreatedAt = blogPost.CreatedAt,
            UpdatedAt = blogPost.UpdatedAt
        };
    }
}
