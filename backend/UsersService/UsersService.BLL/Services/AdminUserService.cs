// UsersService.BLL/Services/AdminUserService.cs

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;
using UsersService.Domain.Entities;

namespace UsersService.BLL.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public AdminUserService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        private async Task<string> GetUserPrimaryRoleAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            // Припускаємо, що нам потрібна лише одна основна роль для відображення
            return roles.FirstOrDefault() ?? "Client"; 
        }

        public async Task<IEnumerable<AdminUserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = _mapper.Map<IEnumerable<AdminUserDto>>(users).ToList();

            // Оскільки роль не мапиться автоматично, отримуємо її вручну
            foreach (var dto in userDtos)
            {
                var user = users.First(u => u.Id == dto.Id);
                dto.Role = await GetUserPrimaryRoleAsync(user);
            }

            return userDtos;
        }
        public async Task<AdminUserDto?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null; // Користувача не знайдено
            }

            // Мапінг сутності на DTO
            var userDto = _mapper.Map<AdminUserDto>(user);

            // Отримання та встановлення ролі вручну
            userDto.Role = await GetUserPrimaryRoleAsync(user);

            return userDto;
        }
        // ... GetUserByIdAsync (аналогічно GetAll, але для одного)

        public async Task<bool> UpdateUserDiscountAsync(string userId, int discount)
        {
            if (discount < 0 || discount > 100) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.PersonalDiscountPercentage = discount;
            var result = await _userManager.UpdateAsync(user);
            
            return result.Succeeded;
        }

        // ... UpdateUserRoleAsync (буде реалізовано пізніше, тут потрібна складніша логіка з Remove/AddRole)
        public Task<bool> UpdateUserRoleAsync(string userId, string newRole)
        {
            throw new NotImplementedException(); 
        }
    }
}