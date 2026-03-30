using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UsersService.BLL.Models;
using UsersService.BLL.Models.Adresess;
using UsersService.BLL.Models.Auth;
using UsersService.BLL.Models.Users;
using UsersService.BLL.Services.Interfaces;
using UsersService.Domain.Entities;

namespace UsersService.API.Controllers
{
    [Authorize]
    [Route("api/me")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser() => Ok(await userService.GetCurrentAsync(UserId));

        [AllowAnonymous]
        [HttpGet("internal/{id}/email")]
        public async Task<IActionResult> GetUserEmailInternal(string id)
        {
            var user = await userService.GetCurrentAsync(id);
            if (user == null || string.IsNullOrEmpty(user.Email)) return NotFound();
            return Ok(new { Email = user.Email });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCurrentUser([FromForm] UpdateUserDto dto) => Ok(await userService.UpdateAsync(UserId, dto));

        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            await userService.ChangePasswordAsync(UserId, dto);
            return Ok(new { Message = "Пароль успішно змінено 🔐" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            await userService.DeleteAsync(UserId);
            return NoContent();
        }
    }
}