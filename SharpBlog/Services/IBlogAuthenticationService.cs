using SharpBlog.Models;
using SharpBlog.Models.DTOs;

namespace SharpBlog.Services;

public interface IBlogAuthenticationService
{
    Task<UserResponseDTO> RegisterUser(RegisterUserDTO registerDTO);
    Task<string> LoginUser(LoginDTO loginDTO);
    Task<UserResponseDTO> UpdateUserDetails(int id, UpdateUserDTO userDto);

}
