using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpBlog.Data.Repository;
using SharpBlog.Models;
using SharpBlog.Models.DTOs;
using System.Security.Claims;

namespace SharpBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogRepo _repo;

        public BlogPostsController(IBlogRepo repo)
        {
            _repo = repo;
        }

        // GET: api/BlogPosts
        /// <summary>
        /// Get a list of blog posts with optional filters and full-text search.
        /// </summary>
        /// <param name="search">Optional keyword to search in title, content, or category</param>
        [HttpGet]
        [Authorize(Roles = "Admin, Author, Reader")]
        public async Task<ActionResult<PagedResult<BlogPostResponseDTO>>> GetBlogPosts(
            [FromQuery] string? author,
            [FromQuery] string? tag,
            [FromQuery] string? category,
            [FromQuery] string? search,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagedPosts = 
                await _repo.GetAllPosts(author, tag, category, search, sortBy, isDescending, pageNumber, pageSize);

            if (pagedPosts.Items == null || !pagedPosts.Items.Any())
            {
                return NotFound("No blog posts found.");
            }

            return Ok(pagedPosts);
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Author, Reader")]
        public async Task<ActionResult<BlogPostResponseDTO>> GetBlogPost(int id)
        {
            var blogPost = await _repo.GetPost(id);
            if (blogPost == null)
            {
                return NotFound($"Blog post with ID {id} not found.");
            }

            return Ok(blogPost);
        }

        [Authorize(Roles = "Author")]
        [HttpGet("me")]
        public async Task<ActionResult<IEnumerable<BlogPostResponseDTO>>> AuthorPosts()
        {
            var blogPosts = await _repo.GetAuthorPosts(GetUserId());
            if (!blogPosts.Any())
                return NotFound("No blog posts found for this author.");
            return Ok(blogPosts);
        }

        [Authorize(Roles = "Author")]
        [HttpGet("me/{id}")]
        public async Task<ActionResult<BlogPostResponseDTO>> AuthorPost(int id)
        {
            var blogPost = await _repo.GetAuthorPost(GetUserId(), id);
            if (blogPost == null)
                return NotFound($"Blog post with ID {id} not found for this author.");
            return Ok(blogPost);
        }

        // POST: api/BlogPosts
        [HttpPost("me")]
        [Authorize(Roles = "Admin, Author")]
        public async Task<ActionResult<BlogPostResponseDTO>> CreateBlogPost(BlogPostDTO blogPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _repo.CreatePost(blogPostDto, GetUserId());

            return CreatedAtAction(nameof(GetBlogPost), new { id = response.Id }, response);
        }

        // PUT: api/BlogPosts/5
        [HttpPut("me/{id}")]
        [Authorize(Roles = "Admin, Author")]
        public async Task<IActionResult> EditBlogPost(int id, BlogPostDTO blogPostDto)
        {

            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _repo.UpdatePost(id, blogPostDto, GetUserId());
                if (!result)
                    return Forbid(); // or NotFound if post doesn't exist            }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("me/{id}")]
        [Authorize(Roles = "Admin, Author")]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            var blogPost = await _repo.DeletePost(id);
            if (blogPost == false)
            {
                return NotFound($"Blog post with ID {id} not found.");
            }

            return NoContent();
        }

        // Private method: Extracts the user's ID from the claims of the current user.
        private int GetUserId()
        {
            var userIdText = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ;
            return int.Parse(userIdText);
        }

    }
}
