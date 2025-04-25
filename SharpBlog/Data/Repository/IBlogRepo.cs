using Microsoft.AspNetCore.Mvc;
using SharpBlog.Models.DTOs;

namespace SharpBlog.Data.Repository;

public interface IBlogRepo
{
    Task<IEnumerable<BlogPostResponseDTO>> GetAllPosts(string? author, string? tag, string? category);
    Task<BlogPostResponseDTO> GetPost(int id);
    Task<BlogPostResponseDTO> CreatePost(BlogPostDTO blogPostDto);

    Task UpdatePost(int id, BlogPostDTO blogPostDto);
    Task<bool> DeletePost(int id);
}
