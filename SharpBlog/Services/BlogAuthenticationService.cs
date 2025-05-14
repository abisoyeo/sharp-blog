using Microsoft.CodeAnalysis.Scripting;
using Microsoft.IdentityModel.Tokens;
using SharpBlog.Data.Repository;
using Microsoft.AspNetCore.Authorization;

using SharpBlog.Models;
using SharpBlog.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SharpBlog.Services;

public class BlogAuthenticationService : IBlogAuthenticationService
{
    private readonly IUserRepo _userRepo;
    private readonly IConfiguration _configuration;

    public BlogAuthenticationService(IUserRepo userRepo, IConfiguration configuration)
    {
        _configuration = configuration;
        _userRepo = userRepo;
    }

    public async Task<User> RegisterUser(RegisterUserDTO userDto)
    {
        var existingUser = await _userRepo.GetUserByEmail(userDto.Email.Trim().ToLower());

        if (existingUser != null)
        {
            throw new ArgumentException("A user with this email already exists.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

        var user = new User
        {
            Name = userDto.Name,
            Email = userDto.Email.Trim().ToLower(),
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            Bio = userDto.Bio,
            ProfilePictureUrl = userDto.ProfilePictureUrl,
            Role = Roles.Role.Author
        };

        await _userRepo.CreateUser(user);
        return user;
    }

    public async Task<string> LoginUser(LoginDTO loginDTO)
    {
        var user = await _userRepo.GetUserByEmail(loginDTO.Email.Trim().ToLower());
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
        {
            // Return null to indicate invalid credentials
            return null;
        }

        return GenerateToken(user);  // Return the generated token if credentials are valid
    }

    public async Task<UserResponseDTO> UpdateUserDetails(int id, UpdateUserDTO userDto)
    {
        var user = await _userRepo.GetUserById(id);
        if (user == null) return null;

        var existingUser = await _userRepo.GetUserByEmail(userDto.Email.Trim().ToLower());

        if (existingUser != null)
        {
            throw new ArgumentException("A user with this email already exists.");
        }

        user.Name = userDto.Name ?? user.Name;
        user.Email = userDto.Email.Trim().ToLower() ?? user.Email;
        user.Bio = userDto.Bio ?? user.Bio;
        user.ProfilePictureUrl = userDto.ProfilePictureUrl ?? user.ProfilePictureUrl;

        if (!string.IsNullOrWhiteSpace(userDto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
        }

        await _userRepo.UpdateUser(user);

        return new UserResponseDTO
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
        };
    }


    public string GenerateToken(User user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:SecretKey"]));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new()
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Email),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            _configuration["Authentication:Issuer"],
            _configuration["Authentication:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
