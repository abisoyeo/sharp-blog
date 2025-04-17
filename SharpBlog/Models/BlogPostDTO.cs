using System.ComponentModel.DataAnnotations;

namespace SharpBlog.Models;

public class BlogPostDTO
{
    [Required, StringLength(100)]
    public string Title { get; set; }
    [Required]
    public string Content { get; set; }
    public string Category { get; set; }
    public string Tags { get; set; }
}
