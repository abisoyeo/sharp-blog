using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharpBlog.Data;
using SharpBlog.Models;
using SharpBlog.Models.DTOs;

namespace SharpBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly BlogDbContext _context;

        public BlogPostsController(BlogDbContext context)
        {
            _context = context;
        }

        // GET: api/BlogPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPostResponseDTO>>> GetBlogPosts()
        {
            var data = await _context.BlogPosts.ToListAsync();

            if (data == null || !data.Any())
            {
                return NotFound("No blog posts found.");
            }

            var blogPosts = data.Select(blogPost => new BlogPostResponseDTO
            {
                Title = blogPost.Title,
                Content = blogPost.Content,
                AuthorName = blogPost.Author.Name,
                Tags = blogPost.Tags,
                Category = blogPost.Category,
                CreatedAt = blogPost.CreatedAt,
                UpdatedAt = blogPost.UpdatedAt
            }).ToList();

            return Ok(blogPosts);
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPostResponseDTO>> GetBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            var blogPostResponse = new BlogPostResponseDTO
            {
                Title = blogPost.Title,
                Content = blogPost.Content,
                AuthorName = blogPost.Author.Name,
                Tags = blogPost.Tags,
                Category = blogPost.Category
            };

            return blogPostResponse;
        }

        // PUT: api/BlogPosts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogPost(int id, BlogPostDTO blogPostDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var author = await _context.Authors.FindAsync(blogPostDto.AuthorId);

            if (author == null)
            {
                return NotFound($"Author with ID {blogPostDto.AuthorId} not found.");
            }

            var updateBlogPost = await _context.BlogPosts.FindAsync(id);

            if (id != updateBlogPost.Id)
            {
                return BadRequest();
            }

            updateBlogPost.Author = author;
            updateBlogPost.Category = blogPostDto.Category;
            updateBlogPost.Content = blogPostDto.Content;
            updateBlogPost.Tags = blogPostDto.Tags;
            updateBlogPost.Title = blogPostDto.Title;
            updateBlogPost.UpdatedAt = DateTime.UtcNow;

            _context.Entry(updateBlogPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BlogPosts
        [HttpPost]
        public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPostDTO blogPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var author = await _context.Authors.FindAsync(blogPostDto.AuthorId);
            if (author == null)
            {
                return NotFound($"Author with ID {blogPostDto.AuthorId} not found.");
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

            return CreatedAtAction(nameof(GetBlogPost), new { id = newBlogPost.Id }, new BlogPostResponseDTO
            {
                Title = newBlogPost.Title,
                Content = newBlogPost.Content,
                AuthorName = author.Name,
                Tags = newBlogPost.Tags,
                Category = newBlogPost.Category
            });
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
