using System.Collections.Generic;
using UsersService.BLL.Models.Users;

namespace UsersService.BLL.Services.Interfaces
{
    public interface IAdminUserService
    {
        Task<IEnumerable<AdminUserDto>> GetAllUsersAsync(UsersFilterDto filter);
        Task<AdminUserDto?> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserDiscountAsync(string userId, int discount);
    }
}