using SharpBlog.Models.DTOs;

namespace SharpBlog.Data.Repository;

public interface IBlogRepo
{
    Task<IEnumerable<BlogPostResponseDTO>> GetAllPosts();
    Task<BlogPostResponseDTO> GetPost(int id);
    Task<BlogPostResponseDTO> CreatePost(BlogPostDTO blogPostDto);

    Task UpdatePost(int id, BlogPostDTO blogPostDto);
    Task<bool> DeletePost(int id);
    Task<IEnumerable<BlogPostResponseDTO>> GetPostsByAuthor(int authorId, string authorName);
    Task<IEnumerable<BlogPostResponseDTO>> GetPostsByCategory(string category);
    Task<IEnumerable<BlogPostResponseDTO>> GetPostsByTag(string tag);
}
