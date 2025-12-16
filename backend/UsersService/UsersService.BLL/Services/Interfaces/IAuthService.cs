using UsersService.BLL.Models.Auth;
using UsersService.BLL.Models.Users;

namespace UsersService.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> RegisterAsync(RegistrationDto model);
        Task<TokenResponseDto?> LoginAsync(LoginDto model);
        Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<bool> LogoutAsync(string refreshToken);
        Task ResendConfirmationEmailAsync(string email);
        Task SendResetPasswordLinkAsync(string email);
        Task ResetPasswordAsync(ResetPasswordDto model);
        Task ConfirmEmailAsync(string userId, string token);
        Task<TokenResponseDto> UpdateUserAsync(string userId, UpdateUserDto dto);
    }
}