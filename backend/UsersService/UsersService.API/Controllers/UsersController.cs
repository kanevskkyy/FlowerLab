using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;
using UsersService.Domain.Entities;

namespace UsersService.API.Controllers
{
    [Authorize] // Всі методи вимагають дійсного Access Token!
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;

        public UsersController(UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }

        /// <summary>
        /// Отримати дані поточного користувача (Personal Information)
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
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

        /// <summary>
        /// Оновити персональні дані (Personal Information)
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromForm] UpdateUserDto updateModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var tokenResponse = await _authService.UpdateUserAsync(userId, updateModel);
                return Ok(tokenResponse);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Ендпоїнт для видалення облікового запису
        /// </summary>
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(new { Message = "Не вдалося видалити обліковий запис." });

            return NoContent();
        }
    }
}