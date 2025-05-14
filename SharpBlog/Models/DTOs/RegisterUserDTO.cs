using System.ComponentModel.DataAnnotations;

namespace SharpBlog.Models.DTOs;

public class RegisterUserDTO
{
    public string Name { get; set; }
    [Required(ErrorMessage = "Username is required.")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public string? ProfilePictureUrl { get;  set; }
    public string? Bio { get;  set; }
}
