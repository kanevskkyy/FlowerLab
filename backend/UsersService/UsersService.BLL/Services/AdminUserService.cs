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
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                query = query.Where(u => u.Email.ToLower().Contains(filter.Email.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
            {
                query = query.Where(u => u.PhoneNumber.ToLower() != null && u.PhoneNumber.ToLower().Contains(filter.PhoneNumber.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(u => 
                    (u.FirstName != null && u.FirstName.ToLower().Contains(term)) || 
                    (u.LastName != null && u.LastName.ToLower().Contains(term)));
            }

            var users = await query.ToListAsync();
    
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
    }
}