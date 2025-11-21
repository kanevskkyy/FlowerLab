using UsersService.BLL.Models;

namespace UsersService.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> RegisterAsync(RegistrationDto model);
        Task<TokenResponseDto?> LoginAsync(LoginDto model);
        Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<bool> LogoutAsync(string refreshToken);
        Task<TokenResponseDto> UpdateUserAsync(string userId, UpdateUserDto dto);
    }
}