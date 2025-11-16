using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;
using UsersService.DAL.DbContext;
using UsersService.Domain.Entities;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace UsersService.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _dbContext;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserManager<ApplicationUser> userManager, IJwtService jwtService, ApplicationDbContext dbContext, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _dbContext = dbContext;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<TokenResponseDto?> RegisterAsync(RegistrationDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return null; 
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
                return null; 
            }
            
            await _userManager.AddToRoleAsync(user, "Client");
            
            return await GenerateAndSaveTokens(user);
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return null;
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
                return false;
            }

            // Анулювання
            storedToken.IsRevoked = true;
            await _dbContext.SaveChangesAsync();

            return true;
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