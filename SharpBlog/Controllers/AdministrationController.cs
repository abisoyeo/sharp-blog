using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SharpBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdministrationController : ControllerBase
    {

        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            // This is a placeholder for the actual implementation.
            // You would typically call a service or repository to get the data.
            var users = new List<string> { "User1", "User2", "User3" };
            return Ok(users);
        }

        [HttpGet]
        [Route("GetUser/{id}")]
        public IActionResult GetUser(int id)
        {
            // This is a placeholder for the actual implementation.
            // You would typically call a service or repository to get the data.
            var user = $"User{id}";
            return Ok(user);
        }

        [HttpPost]
        [Route("BanUser/{id}")]
        public IActionResult BanUser(int id)
        {
            // This is a placeholder for the actual implementation.
            // You would typically call a service or repository to get the data.
            return Ok($"User{id} has been banned.");
        }

        [HttpPost]
        [Route("UnbanUser/{id}")]
        public IActionResult UnbanUser(int id)
        {
            // This is a placeholder for the actual implementation.
            // You would typically call a service or repository to get the data.
            return Ok($"User{id} has been unbanned.");
        }

        [HttpPost]
        [Route("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            // This is a placeholder for the actual implementation.
            // You would typically call a service or repository to get the data.
            return Ok($"User{id} has been deleted.");
        }

        //[HttpPut("role")] 
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
