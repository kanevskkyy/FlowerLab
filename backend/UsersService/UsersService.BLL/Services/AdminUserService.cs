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
            return roles.FirstOrDefault() ?? "Client"; 
        }

        public async Task<IEnumerable<AdminUserDto>> GetAllUsersAsync(UsersFilterDto filter)
        {
            // 1. Починаємо будувати запит, але ще НЕ виконуємо його
            var query = _userManager.Users.AsQueryable();

            // 2. Застосовуємо фільтри, якщо вони передані

            // Фільтр по Email (частковий збіг)
            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                query = query.Where(u => u.Email.Contains(filter.Email));
            }

            // Фільтр по Телефону (частковий збіг)
            if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
            {
                query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(filter.PhoneNumber));
            }

            // Загальний пошук по Імені або Прізвищу
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                // Шукаємо збіг в FirstName АБО LastName
                query = query.Where(u => 
                    (u.FirstName != null && u.FirstName.ToLower().Contains(term)) || 
                    (u.LastName != null && u.LastName.ToLower().Contains(term)));
            }

            // 3. Виконуємо запит до БД тільки тут (ToListAsync)
            var users = await query.ToListAsync();
    
            // 4. Мапінг (залишається як було)
            var userDtos = _mapper.Map<IEnumerable<AdminUserDto>>(users).ToList();

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
                return null; 
            }

            var userDto = _mapper.Map<AdminUserDto>(user);

            userDto.Role = await GetUserPrimaryRoleAsync(user);

            return userDto;
        }

        public async Task<bool> UpdateUserDiscountAsync(string userId, int discount)
        {
            if (discount < 0 || discount > 100) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.PersonalDiscountPercentage = discount;
            var result = await _userManager.UpdateAsync(user);
            
            return result.Succeeded;
        }

        public Task<bool> UpdateUserRoleAsync(string userId, string newRole)
        {
            throw new NotImplementedException(); 
        }
    }
}