using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Errors.Model;
using System.Linq;
using System.Threading.Tasks;
using UsersService.BLL.EmailService;
using UsersService.BLL.EmailService.DTO;
using UsersService.BLL.Helpers;
using UsersService.BLL.Models.Auth;
using UsersService.BLL.Models.Users;
using UsersService.BLL.Services.Interfaces;
using UsersService.DAL.DbContext;
using UsersService.Domain.Entities;

namespace UsersService.BLL.Services
{
    public class AuthService : IAuthService
    {
        private UserManager<ApplicationUser> userManager;
        private IJwtService jwtService;
        private ApplicationDbContext dbContext;
        private JwtSettings jwtSettings;
        private IUserImageService imageService;
        private IEmailService emailService;
        private IConfiguration configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager, 
            IJwtService jwtService,
            ApplicationDbContext dbContext,
            IOptions<JwtSettings> jwtSettings,
            IUserImageService userImageService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            imageService = userImageService;
            this.userManager = userManager;
            this.jwtService = jwtService;
            this.dbContext = dbContext;
            this.jwtSettings = jwtSettings.Value;
            this.emailService = emailService;
            this.configuration = configuration;
        }


        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"Користувача з ID '{userId}' не знайдено.");

            var result = await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Не вдалося змінити пароль: {errors}");
            }

            return true;
        }

        public async Task<TokenResponseDto?> RegisterAsync(RegistrationDto model)
        {
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Користувач з email '{model.Email}' вже існує.");
            }

            var phoneExists = await userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
            if (phoneExists)
            {
                throw new InvalidOperationException($"Користувач з номером '{model.PhoneNumber}' вже існує.");
            }

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Створення користувача не вдалося: {errors}");
            }

            await userManager.AddToRoleAsync(user, "Client");


            string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            string encodedToken = Uri.EscapeDataString(token);

            string confirmUrl =
                $"{configuration["Frontend:ConfirmEmailUrl"]}?userId={user.Id}&token={encodedToken}";

            await emailService.SendEmailAsync(new EmailMessageDTO
            {
                To = user.Email,
                Subject = "Підтвердження email 📧",
                HtmlBody = $"""
                    <h2>Підтвердження email</h2>
                    <p>Привіт, {user.FirstName} 👋</p>
                    <p>Натисни кнопку, щоб підтвердити email:</p>
                    <a href="{confirmUrl}">
                        Підтвердити email
                    </a>
                """
            });

            return null;

        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
                throw new InvalidOperationException($"Користувач з email '{model.Email}' не існує.");

            if (await userManager.IsLockedOutAsync(user))
                throw new UnauthorizedAccessException("Акаунт тимчасово заблоковано через велику кількість невдалих входів.");

            var passwordValid = await userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordValid)
            {
                await userManager.AccessFailedAsync(user);

                throw new UnauthorizedAccessException("Невірний пароль або email.");
            }

            await userManager.ResetAccessFailedCountAsync(user);

            if (!user.EmailConfirmed)
                throw new UnauthorizedAccessException("Підтвердіть email!");

            return await GenerateAndSaveTokens(user);
        }

        public async Task ConfirmEmailAsync(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("Користувача не знайдено!");

            var decodedToken = Uri.UnescapeDataString(token);
            var result = await userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Не вдалося підтвердити email: {errors}");
            }
        }


        public async Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await dbContext.RefreshTokens
                .Include(t => t.User)
                .SingleOrDefaultAsync(t => t.Token == refreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return null;
            }

            var user = storedToken.User;
            storedToken.IsRevoked = true;

            return await GenerateAndSaveTokens(user);
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var storedToken = await dbContext.RefreshTokens
                .SingleOrDefaultAsync(t => t.Token == refreshToken);

            if (storedToken == null)
            {
                throw new InvalidOperationException("Токен оновлення не знайдено.");
            }

            storedToken.IsRevoked = true;
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<TokenResponseDto> UpdateUserAsync(string userId, UpdateUserDto dto)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"Користувач з ID '{userId}' не існує.");

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
                var phoneExists = await userManager.Users
                    .AnyAsync(u => u.PhoneNumber == dto.PhoneNumber && u.Id != user.Id);

                if (phoneExists)
                    throw new InvalidOperationException($"Номер телефону '{dto.PhoneNumber}' вже використовується іншим користувачем.");

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
                        await imageService.DeleteAsync(publicId);
                    }
                    catch { }
                }

                var url = await imageService.UploadAsync(dto.Photo, "users/photos");
                user.PhotoUrl = url;
                isUpdated = true;
            }

            if (!isUpdated)
            {
                return await GenerateAndSaveTokens(user);
            }

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Не вдалося оновити користувача: {errors}");
            }

            return await GenerateAndSaveTokens(user);
        }

        private string GetPublicIdFromUrl(string url)
        {
            var fileName = url.Split('/').Last();
            return fileName.Split('.').First();
        }

        public async Task ResendConfirmationEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                throw new NotFoundException("Користувача не знайдено!");

            if (user.EmailConfirmed)
                throw new InvalidOperationException("Email вже підтверджений ✅");

            string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            string encodedToken = Uri.EscapeDataString(token);

            string confirmUrl =
                $"{configuration["Frontend:ConfirmEmailUrl"]}?userId={user.Id}&token={encodedToken}";

            await emailService.SendEmailAsync(new EmailMessageDTO
            {
                To = user.Email,
                Subject = "Підтвердження email",
                HtmlBody = $"""
                    <h2>Підтвердження email</h2>
                    <p>Привіт, {user.FirstName} 👋</p>
                    <p>Натисни кнопку, щоб підтвердити email:</p>
                    <a href="{confirmUrl}">
                        Підтвердити email
                    </a>
                """
            });
        }

        public async Task SendResetPasswordLinkAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                throw new NotFoundException("Користувача з таким email не знайдено!");

            string token = await userManager.GeneratePasswordResetTokenAsync(user);
            string encodedToken = Uri.EscapeDataString(token);

            string resetUrl = $"{configuration["Frontend:ResetPasswordUrl"]}?userId={user.Id}&token={encodedToken}";

            await emailService.SendEmailAsync(new EmailMessageDTO
            {
                To = user.Email,
                Subject = "Скидання пароля",
                HtmlBody = $"""
                    <h2>Скидання пароля</h2>
                    <p>Привіт, {user.FirstName} 👋</p>
                    <p>Натисни кнопку, щоб змінити пароль:</p>
                    <a href="{resetUrl}">
                        Змінити пароль
                    </a>
                """
            });
        }

        public async Task ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
                throw new NotFoundException("Користувача не знайдено!");

            var decodedToken = Uri.UnescapeDataString(model.Token);
            var result = await userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Не вдалося скинути пароль: {errors}");
            }
        }

        private async Task<TokenResponseDto> GenerateAndSaveTokens(ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var tokenData = jwtService.CreateTokens(user, roles);

            var oldTokens = dbContext.RefreshTokens.Where(t => t.UserId == user.Id && !t.IsRevoked);
            dbContext.RefreshTokens.RemoveRange(oldTokens);

            var refreshTokenEntity = new RefreshToken
            {
                Token = tokenData.RefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays),
                UserId = user.Id,
                IsRevoked = false
            };

            await dbContext.RefreshTokens.AddAsync(refreshTokenEntity);
            await dbContext.SaveChangesAsync();

            return tokenData;
        }
    }
}