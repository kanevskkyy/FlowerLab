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
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser() => Ok(await userService.GetCurrentAsync(UserId));

        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromForm] UpdateUserDto dto) => Ok(await userService.UpdateAsync(UserId, dto));

        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            await userService.ChangePasswordAsync(UserId, dto);
            return Ok(new { Message = "Пароль успішно змінено 🔐" });
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteAccount()
        {
            await userService.DeleteAsync(UserId);
            return NoContent();
        }
    }
}