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

            var user = await userManager.Users
                .Include(u => u.Addresses) 
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            var userDto = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PhotoUrl = user.PhotoUrl,
                Addresses = user.Addresses.Select(a => new AddressDto
                {
                    Id = a.Id,
                    Country = a.Country,
                    City = a.City,
                    Street = a.Street,
                    PostalCode = a.PostalCode,
                    IsDefault = a.IsDefault
                }).ToList()
            };

            return Ok(userDto);
        }

        [HttpGet("me/addresses")]
        public async Task<IActionResult> GetMyAddresses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var addresses = user.Addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                Country = a.Country,
                City = a.City,
                Street = a.Street,
                PostalCode = a.PostalCode,
                IsDefault = a.IsDefault
            }).ToList();

            return Ok(addresses);
        }

        [HttpPost("me/addresses")]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var address = new Address
            {
                UserId = user.Id,
                Country = dto.Country,
                City = dto.City,
                Street = dto.Street,
                PostalCode = dto.PostalCode,
                IsDefault = dto.IsDefault
            };

            user.Addresses.Add(address);
            await userManager.UpdateAsync(user);

            return Ok(new AddressDto
            {
                Id = address.Id,
                Country = address.Country,
                City = address.City,
                Street = address.Street,
                PostalCode = address.PostalCode,
                IsDefault = address.IsDefault
            });
        }

        [HttpPut("me/addresses/{addressId}")]
        public async Task<IActionResult> UpdateAddress(Guid addressId, [FromBody] CreateAddressDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null) return NotFound();

            address.Country = dto.Country;
            address.City = dto.City;
            address.Street = dto.Street;
            address.PostalCode = dto.PostalCode;
            address.IsDefault = dto.IsDefault;

            await userManager.UpdateAsync(user);

            return NoContent();
        }

        [HttpDelete("me/addresses/{addressId}")]
        public async Task<IActionResult> DeleteAddress(Guid addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null) return NotFound();

            user.Addresses.Remove(address);
            await userManager.UpdateAsync(user);

            return NoContent();
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