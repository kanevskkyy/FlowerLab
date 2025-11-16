using UsersService.BLL.Models;

namespace UsersService.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> RegisterAsync(RegistrationDto model);
        Task<TokenResponseDto?> LoginAsync(LoginDto model);
        Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
    }
}