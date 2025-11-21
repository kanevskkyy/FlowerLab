using UsersService.BLL.Models;
using System.Collections.Generic;

namespace UsersService.BLL.Interfaces
{
    public interface IAdminUserService
    {
        Task<IEnumerable<AdminUserDto>> GetAllUsersAsync(UsersFilterDto filter);
        Task<AdminUserDto?> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserDiscountAsync(string userId, int discount);
    }
}