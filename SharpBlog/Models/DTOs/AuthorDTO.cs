using System.ComponentModel.DataAnnotations;

namespace SharpBlog.Models.DTOs
{
    public class AuthorDTO
    {
        [Required, StringLength(50)]
        public string Name { get; set; }
        public string? Bio { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? ProfilePictureUrl { get; set; } = null;
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
}
