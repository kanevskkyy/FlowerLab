using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.BLL.Models.Users;
using UsersService.BLL.Services.Interfaces;

namespace UsersService.API.Controllers
{
    [Authorize(Roles = "Admin")] 
    [Route("api/admin/users")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private IAdminUserService adminUserService;

        public AdminUsersController(IAdminUserService adminUserService)
        {
            this.adminUserService = adminUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UsersFilterDto filter)
        {
            var users = await adminUserService.GetAllUsersAsync(filter);
            return Ok(users);
        }

        [HttpPut("{userId}/discount")]
        public async Task<IActionResult> UpdateUserDiscount(string userId, [FromBody] UpdateDiscountDto model)
        {
            var result = await adminUserService.UpdateUserDiscountAsync(userId, model.PersonalDiscountPercentage);

            if (!result)
            {
                return NotFound(); 
            }

            return Ok(new { Message = $"Знижка для користувача {userId} оновлена до {model.PersonalDiscountPercentage}%." });
        }

    }
}