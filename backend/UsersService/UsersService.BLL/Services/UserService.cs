using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.Exceptions;
using UsersService.BLL.Models.Adresess;
using UsersService.BLL.Models.Auth;
using UsersService.BLL.Models.Users;
using UsersService.BLL.Services.Interfaces;
using UsersService.Domain.Entities;
using UsersService.DAL.DbContext;

namespace UsersService.BLL.Services
{
    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> userManager;
        private IAuthService authService;
        private ApplicationDbContext _context;

        public UserService(
            UserManager<ApplicationUser> userManager,
            IAuthService authService,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.authService = authService;
            this._context = context;
        }

        public async Task<UserResponseDto> GetCurrentAsync(string userId)
        {
            var user = await userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new NotFoundException("User not found");

            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PhotoUrl = user.PhotoUrl,
                PersonalDiscountPercentage = user.PersonalDiscountPercentage,
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
            try 
            {
                var user = await userManager.Users
                    .Include(u => u.Addresses)
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.Id == userId)
                    ?? throw new NotFoundException("User not found");

                Console.WriteLine($"[DeleteAsync] Found user {userId}. Addresses: {user.Addresses?.Count}, Tokens: {user.RefreshTokens?.Count}");

                // 1. Remove Roles (Fix for FK_AspNetUserRoles_AspNetUsers_UserId)
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    Console.WriteLine($"[DeleteAsync] Removing roles: {string.Join(", ", roles)}");
                    await userManager.RemoveFromRolesAsync(user, roles);
                }

                // 2. Remove Addresses
                if (user.Addresses != null && user.Addresses.Any())
                {
                    _context.Addresses.RemoveRange(user.Addresses);
                    Console.WriteLine("[DeleteAsync] Removing addresses...");
                }

                // 3. Remove RefreshTokens
                if (user.RefreshTokens != null && user.RefreshTokens.Any())
                {
                    _context.RefreshTokens.RemoveRange(user.RefreshTokens);
                    Console.WriteLine("[DeleteAsync] Removing tokens...");
                }

                await _context.SaveChangesAsync();
                Console.WriteLine("[DeleteAsync] Dependencies saved.");

                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded) 
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    Console.WriteLine($"[DeleteAsync] DeleteAsync failed: {errors}");
                    throw new Exception($"Failed to delete account: {errors}");
                }
                Console.WriteLine("[DeleteAsync] User deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteAsync] ERROR: {ex.Message}");
                Console.WriteLine($"[DeleteAsync] STACK: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[DeleteAsync] INNER: {ex.InnerException.Message}");
                }
                throw;
            }
        }

    }
}
