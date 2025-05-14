using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharpBlog.Models.DTOs;
using SharpBlog.Services;
using System.Security.Claims;

namespace SharpBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IBlogAuthenticationService _authService;

        public AccountController(IBlogAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterUser(userDto);
                if (result == null)
                {
                    return BadRequest("Registration failed.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _authService.LoginUser(loginDto);

            if (token == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpPatch("update")]
        public async Task<ActionResult<UserResponseDTO>> UpdateUserDetails([FromBody] UpdateUserDTO userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get the user ID from the JWT token
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var updatedUser = await _authService.UpdateUserDetails(userId, userDto);

                if (updatedUser == null)
                    return NotFound("User not found.");

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

    }
}
