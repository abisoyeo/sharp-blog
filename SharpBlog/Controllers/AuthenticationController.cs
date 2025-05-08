using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharpBlog.Models.DTOs;
using SharpBlog.Services;

namespace SharpBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterUser(registerDto);
            if (result == null)
            {
                return BadRequest("Registration failed.");
            }

            return Ok(result);
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

        //[HttpPut("role")]  move to a user controller
        //public async Task<IActionResult> Role([FromBody] RoleDTO roleDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var result = await _authService.UpdateUserRole(roleDto);
        //    if (result == null)
        //    {
        //        return BadRequest("Role update failed.");
        //    }

        //    return Ok(result);
        //}
    }
}
