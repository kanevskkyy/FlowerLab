using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.Models.Auth;
using UsersService.BLL.Models.Users;

namespace UsersService.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetCurrentAsync(string userId);
        Task<TokenResponseDto> UpdateAsync(string userId, UpdateUserDto dto);
        Task ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task DeleteAsync(string userId);
    }
}
