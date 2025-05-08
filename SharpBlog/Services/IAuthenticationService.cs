using SharpBlog.Models;
using SharpBlog.Models.DTOs;

namespace SharpBlog.Services;

public interface IAuthenticationService
{
    Task<User> RegisterUser(RegisterDTO registerDTO);
    Task<string> LoginUser(LoginDTO loginDTO);
}
