using System.ComponentModel.DataAnnotations;

namespace SharpBlog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }

    }
}
