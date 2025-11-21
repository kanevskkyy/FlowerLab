using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;

namespace UsersService.API.Controllers
{
    [Authorize(Roles = "Admin")] 
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
        /// Отримати список користувачів з можливістю фільтрації
        /// </summary>
        /// <param name="filter">Параметри фільтрації (Email, Phone, SearchTerm)</param>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UsersFilterDto filter)
        {
            var users = await _adminUserService.GetAllUsersAsync(filter);
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
                return NotFound(); 
            }

            return Ok(new { Message = $"Знижка для користувача {userId} оновлена до {model.PersonalDiscountPercentage}%." });
        }

    }
}