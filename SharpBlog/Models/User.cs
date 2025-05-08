using static SharpBlog.Models.Roles;

namespace SharpBlog.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; } = null;
    public DateTime CreatedAt { get; set; }
    public ICollection<BlogPost>? BlogPosts { get; set; }
    public Role Role { get; set; }  
}
