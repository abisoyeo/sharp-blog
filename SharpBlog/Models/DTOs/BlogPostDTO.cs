using System.ComponentModel.DataAnnotations;

namespace SharpBlog.Models.DTOs;

public class BlogPostDTO
{
    [Required, StringLength(100)]
    public string Title { get; set; }
    [Required]
    public string Content { get; set; }
    public string Category { get; set; }
    public List<string>? Tags { get; set; }
}
