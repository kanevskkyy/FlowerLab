using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.Models.Adresess;
using UsersService.BLL.Models.Auth;
using UsersService.BLL.Models.Users;
using UsersService.BLL.Services.Interfaces;
using UsersService.Domain.Entities;

namespace UsersService.BLL.Services
{
    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> userManager;
        private IAuthService authService;

        public UserService(
            UserManager<ApplicationUser> userManager,
            IAuthService authService)
        {
            this.userManager = userManager;
            this.authService = authService;
        }

        public async Task<UserResponseDto> GetCurrentAsync(string userId)
        {
            var user = await userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new NotFoundException("Користувача не знайдено");

            return new UserResponseDto
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
                    Address = a.Address,
                    IsDefault = a.IsDefault
                }).ToList()
            };
        }

        public async Task<TokenResponseDto> UpdateAsync(string userId, UpdateUserDto dto)
        {
            return await authService.UpdateUserAsync(userId, dto);
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            await authService.ChangePasswordAsync(userId, dto);
        }

        public async Task DeleteAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException("Користувача не знайдено");

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded) throw new Exception("Не вдалося видалити акаунт");
        }
    }
}
