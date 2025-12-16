using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersService.BLL.Models;
using UsersService.BLL.Models.Auth;
using UsersService.BLL.Models.Users;
using UsersService.BLL.Services.Interfaces;
using UsersService.Domain.Entities;

namespace UsersService.API.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;
        private IAuthService authService;

        public UsersController(UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            this.userManager = userManager;
            this.authService = authService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PhotoUrl = user.PhotoUrl
            });
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromForm] UpdateUserDto updateModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var tokenResponse = await authService.UpdateUserAsync(userId, updateModel);
                return Ok(tokenResponse);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var result = await authService.ChangePasswordAsync(userId, dto);
                return Ok(new { Message = "Пароль успішно змінено." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpDelete("me")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(new { Message = "Не вдалося видалити обліковий запис." });

            return NoContent();
        }
    }
}