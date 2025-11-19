using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;
using UsersService.DAL.DbContext;
using UsersService.Domain.Entities;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UsersService.BLL.Helpers;

namespace UsersService.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _dbContext;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserImageService _imageService;


        public AuthService(UserManager<ApplicationUser> userManager, IJwtService jwtService, ApplicationDbContext dbContext, IOptions<JwtSettings> jwtSettings, IUserImageService userImageService)
        {
            _imageService = userImageService;
            _userManager = userManager;
            _jwtService = jwtService;
            _dbContext = dbContext;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<TokenResponseDto?> RegisterAsync(RegistrationDto model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email '{model.Email}' already exists.");
            }

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            await _userManager.AddToRoleAsync(user, "Client");

            return await GenerateAndSaveTokens(user);
        }


        public async Task<TokenResponseDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                throw new InvalidOperationException($"User with email '{model.Email}' does not exist.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                throw new UnauthorizedAccessException("Invalid password or email.");
            }

            return await GenerateAndSaveTokens(user);
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            // 1. Знайти токен у БД
            var storedToken = await _dbContext.RefreshTokens
                .Include(t => t.User)
                .SingleOrDefaultAsync(t => t.Token == refreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return null; // Токен не знайдено, анульований або прострочений
            }

            // 2. Генерація нових токенів
            var user = storedToken.User;
    
            // 3. Анулювати старий токен і створити новий
            storedToken.IsRevoked = true;
    
            // 4. Згенерувати та зберегти новий
            return await GenerateAndSaveTokens(user); 
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            // Знайти токен у БД
            var storedToken = await _dbContext.RefreshTokens
                .SingleOrDefaultAsync(t => t.Token == refreshToken);

            if (storedToken == null)
            {
                throw new InvalidOperationException("Refresh token not found.");
            }

            // Анулювання
            storedToken.IsRevoked = true;
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<TokenResponseDto> UpdateUserAsync(string userId, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID '{userId}' does not exist.");

            bool isUpdated = false;

            if (!string.IsNullOrEmpty(dto.FirstName) && dto.FirstName != user.FirstName)
            {
                user.FirstName = dto.FirstName;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(dto.LastName) && dto.LastName != user.LastName)
            {
                user.LastName = dto.LastName;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(dto.PhoneNumber) && dto.PhoneNumber != user.PhoneNumber)
            {
                user.PhoneNumber = dto.PhoneNumber;
                isUpdated = true;
            }

            if (dto.Photo != null)
            {
                if (!string.IsNullOrEmpty(user.PhotoUrl))
                {
                    try
                    {
                        var publicId = GetPublicIdFromUrl(user.PhotoUrl);
                        await _imageService.DeleteAsync(publicId);
                    }
                    catch { }
                }

                var url = await _imageService.UploadAsync(dto.Photo, "users/photos");
                user.PhotoUrl = url;
                isUpdated = true;
            }

            if (!isUpdated)
            {
                // Якщо нічого не змінено, просто повертаємо існуючі токени
                return await GenerateAndSaveTokens(user);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to update user: {errors}");
            }

            // Після успішного оновлення — перегенеруємо токени
            return await GenerateAndSaveTokens(user);
        }


        private string GetPublicIdFromUrl(string url)
        {
            var fileName = url.Split('/').Last();
            return fileName.Split('.').First();
        }

        private async Task<TokenResponseDto> GenerateAndSaveTokens(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            var tokenData = _jwtService.CreateTokens(user, roles);

            // Анулюємо старі токени, щоб уникнути їх накопичення (опціонально, але рекомендовано)
            var oldTokens = _dbContext.RefreshTokens.Where(t => t.UserId == user.Id && !t.IsRevoked);
            _dbContext.RefreshTokens.RemoveRange(oldTokens);
            
            var refreshTokenEntity = new RefreshToken
            {
                Token = tokenData.RefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays), 
                UserId = user.Id,
                IsRevoked = false
            };
            
            await _dbContext.RefreshTokens.AddAsync(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            return tokenData;
        }
    }
}