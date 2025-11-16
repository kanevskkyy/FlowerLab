// UsersService.API/Controllers/AdminUsersController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;

namespace UsersService.API.Controllers
{
    [Authorize(Roles = "Admin")] // Тільки Admin має доступ
    [Route("api/admin/users")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUsersController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        /// <summary>
        /// Отримати список усіх користувачів
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminUserService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Встановити персональну знижку для користувача
        /// </summary>
        [HttpPut("{userId}/discount")]
        public async Task<IActionResult> UpdateUserDiscount(string userId, [FromBody] UpdateDiscountDto model)
        {
            var result = await _adminUserService.UpdateUserDiscountAsync(userId, model.PersonalDiscountPercentage);

            if (!result)
            {
                return NotFound(); // Якщо користувача не знайдено
            }

            return Ok(new { Message = $"Discount for user {userId} updated to {model.PersonalDiscountPercentage}%." });
        }
    }
}