using Microsoft.AspNetCore.Mvc;
using SharpBlog.Models.DTOs;
using System.Globalization;

namespace SharpBlog.Data.Repository;

public interface IBlogRepo
{
    Task<PagedResult<BlogPostResponseDTO>> GetAllPosts(string? author, string? tag, string? category, string? search, string? sortBy, bool isDescending, int pageNumber, int pageSize);
    Task<BlogPostResponseDTO> GetPost(int id);
    Task<BlogPostResponseDTO> CreatePost(BlogPostDTO blogPostDto, int userId);

    Task<bool> UpdatePost(int postId, BlogPostDTO blogPostDto, int userId); Task<bool> DeletePost(int id);
    Task<IEnumerable<BlogPostResponseDTO>> GetAuthorPosts(int userId);
    Task<BlogPostResponseDTO> GetAuthorPost(int userId, int blogId);
}
