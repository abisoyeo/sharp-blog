using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpBlog.Data.Repository;
using SharpBlog.Models;
using SharpBlog.Models.DTOs;

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
        [HttpGet]
        [Authorize(Roles = "Admin, Author, Reader")] // All roles can read blog posts
        public async Task<ActionResult<IEnumerable<BlogPostResponseDTO>>> GetBlogPosts(
            [FromQuery] string? author,
            [FromQuery] string? tag,
            [FromQuery] string? category)
        {
            var blogPosts = await _repo.GetAllPosts(author, tag, category);

            if (blogPosts == null)
            {
                return NotFound("No blog posts found.");
            }

            return Ok(blogPosts);
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Author, Reader")] // All roles can read blog posts
        public async Task<ActionResult<BlogPostResponseDTO>> GetBlogPost(int id)
        {
            var blogPost = await _repo.GetPost(id);
            if (blogPost == null)
            {
                return NotFound($"Blog post with ID {id} not found.");
            }

            return Ok(blogPost);
        }

        // PUT: api/BlogPosts/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Author")] // Only Admin and Author can edit blog posts
        public async Task<IActionResult> EditBlogPost(int id, BlogPostDTO blogPostDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _repo.UpdatePost(id, blogPostDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        // POST: api/BlogPosts
        [HttpPost]
        [Authorize(Roles = "Admin, Author")] // Only Admin and Author can create blog posts
        public async Task<ActionResult<BlogPost>> CreateBlogPost(BlogPostDTO blogPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _repo.CreatePost(blogPostDto);


            return CreatedAtAction(nameof(GetBlogPost), new { id = response.Id }, response);
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete blog posts
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            var blogPost = await _repo.DeletePost(id);
            if (blogPost == false)
            {
                return NotFound($"Blog post with ID {id} not found.");
            }

            return NoContent();
        }

    }
}
