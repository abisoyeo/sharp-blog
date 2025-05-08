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

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepo _userRepo;
    private readonly IConfiguration _configuration;

    public AuthenticationService(IUserRepo userRepo, IConfiguration configuration)
    {
        _configuration = configuration;
        _userRepo = userRepo;
    }

    public async Task<User> RegisterUser(RegisterDTO registerDTO)
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

        var user = new User
        {
            Name = registerDTO.Name,
            Email = registerDTO.Username,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            Bio = registerDTO.Bio,
            ProfilePictureUrl = registerDTO.ProfilePictureUrl,
            Role = Roles.Role.Author
        };

         _userRepo.Create(user);
        return user;
    }

    public async Task<string> LoginUser(LoginDTO loginDTO)
    {
        var user = _userRepo.GetUser(loginDTO.Username);
        if (user.Email == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return GenerateToken(user);
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
