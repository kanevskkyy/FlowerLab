using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using UsersService.BLL.Helpers;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;
using UsersService.BLL.Services;
using UsersService.DAL.DbContext;
using UsersService.Domain.Entities;

namespace UsersService.Tests.Services
{

    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<ApplicationDbContext> _dbContextMock;
        private readonly Mock<IUserImageService> _imageServiceMock;
        private readonly AuthService _authService;
        private readonly JwtSettings _jwtSettings;

        public AuthServiceTests()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _jwtServiceMock = new Mock<IJwtService>();
            _dbContextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _imageServiceMock = new Mock<IUserImageService>();

            var refreshTokens = new List<RefreshToken>().AsQueryable();
            var mockSet = new Mock<DbSet<RefreshToken>>();
            mockSet.As<IQueryable<RefreshToken>>().Setup(m => m.Provider).Returns(refreshTokens.Provider);
            mockSet.As<IQueryable<RefreshToken>>().Setup(m => m.Expression).Returns(refreshTokens.Expression);
            mockSet.As<IQueryable<RefreshToken>>().Setup(m => m.ElementType).Returns(refreshTokens.ElementType);
            mockSet.As<IQueryable<RefreshToken>>().Setup(m => m.GetEnumerator()).Returns(refreshTokens.GetEnumerator());

            _dbContextMock.Setup(c => c.RefreshTokens).Returns(mockSet.Object);
            _dbContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _jwtSettings = new JwtSettings { RefreshTokenExpirationDays = 7 };

            _authService = new AuthService(
                _userManagerMock.Object,
                _jwtServiceMock.Object,
                _dbContextMock.Object,
                Options.Create(_jwtSettings),
                _imageServiceMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenEmailExists()
        {
            // Arrange
            var dto = new RegistrationDto { Email = "test@test.com", Password = "Password123!", FirstName = "John", LastName = "Doe" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(dto.Email))
                            .ReturnsAsync(new ApplicationUser());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(dto));
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnToken_WhenUserCreated()
        {
            // Arrange
            var dto = new RegistrationDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "1234567890"
            };

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = dto.Email,
                UserName = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };

            // Мокаємо UserManager
            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((ApplicationUser)null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<ApplicationUser, string>((u, p) => u.Id = user.Id);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Client"))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "Client" });

            // Мокаємо RefreshTokens DbSet
            var refreshTokensData = new List<RefreshToken>().AsQueryable();
            var refreshTokensMock = new Mock<DbSet<RefreshToken>>();
            refreshTokensMock.As<IQueryable<RefreshToken>>().Setup(m => m.Provider).Returns(refreshTokensData.Provider);
            refreshTokensMock.As<IQueryable<RefreshToken>>().Setup(m => m.Expression).Returns(refreshTokensData.Expression);
            refreshTokensMock.As<IQueryable<RefreshToken>>().Setup(m => m.ElementType).Returns(refreshTokensData.ElementType);
            refreshTokensMock.As<IQueryable<RefreshToken>>().Setup(m => m.GetEnumerator()).Returns(refreshTokensData.GetEnumerator());
            _dbContextMock.Setup(db => db.RefreshTokens).Returns(refreshTokensMock.Object);

            // Мокаємо JWT Service
            _jwtServiceMock.Setup(s => s.CreateTokens(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
                .Returns(new TokenResponseDto
                {
                    AccessToken = "access-token",
                    RefreshToken = "refresh-token",
                    AccessTokenExpiration = DateTime.UtcNow.AddMinutes(15)
                });

            _dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _authService.RegisterAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("access-token", result.AccessToken);
            Assert.Equal("refresh-token", result.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
        {
            var dto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
            var user = new ApplicationUser { Email = dto.Email };

            _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email))
                .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, dto.Password))
                .ReturnsAsync(true);
            _jwtServiceMock.Setup(j => j.CreateTokens(user, It.IsAny<IList<string>>()))
                .Returns(new TokenResponseDto { AccessToken = "access", RefreshToken = "refresh", AccessTokenExpiration = DateTime.UtcNow.AddMinutes(15) });

            var result = await _authService.LoginAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("access", result.AccessToken);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldThrow_WhenUserNotFound()
        {
            _userManagerMock.Setup(um => um.FindByIdAsync("123")).ReturnsAsync((ApplicationUser)null);

            var dto = new ChangePasswordDto { OldPassword = "old", NewPassword = "new", ConfirmPassword = "new" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ChangePasswordAsync("123", dto));
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnTrue_WhenSuccess()
        {
            var user = new ApplicationUser { Id = "123" };
            _userManagerMock.Setup(um => um.FindByIdAsync("123")).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.ChangePasswordAsync(user, "old", "new"))
                            .ReturnsAsync(IdentityResult.Success);

            var dto = new ChangePasswordDto { OldPassword = "old", NewPassword = "new", ConfirmPassword = "new" };

            var result = await _authService.ChangePasswordAsync("123", dto);

            Assert.True(result);
        }
    }

}
