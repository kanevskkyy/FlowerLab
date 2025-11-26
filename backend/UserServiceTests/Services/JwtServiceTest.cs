using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.Models;
using UsersService.BLL.Services;
using UsersService.Domain.Entities;
using Xunit;
using Microsoft.Extensions.Options;

namespace UsersService.Tests.Services
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            var settings = Options.Create(new JwtSettings
            {
                Secret = "supersecretkey1234567890supersecret!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenExpirationMinutes = 60
            });

            _jwtService = new JwtService(settings);
        }

        [Fact]
        public void CreateTokens_ShouldReturnAccessAndRefreshTokens()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "user1",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "123456789",
                PersonalDiscountPercentage = 10,
                PhotoUrl = "https://example.com/photo.jpg"
            };
            var roles = new List<string> { "Admin", "User" };

            // Act
            var result = _jwtService.CreateTokens(user, roles);

            // Assert
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));
            Assert.True(result.AccessTokenExpiration > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnUniqueToken()
        {
            // Act
            var token1 = _jwtService.GenerateRefreshToken();
            var token2 = _jwtService.GenerateRefreshToken();

            // Assert
            Assert.False(string.IsNullOrEmpty(token1));
            Assert.False(string.IsNullOrEmpty(token2));
            Assert.NotEqual(token1, token2); // Майже завжди різні, оскільки RNG
        }

        [Fact]
        public void CreateTokens_ShouldContainCorrectClaims()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "user1",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "123456789",
                PersonalDiscountPercentage = 10,
                PhotoUrl = "https://example.com/photo.jpg"
            };
            var roles = new List<string> { "Admin" };

            // Act
            var tokenResponse = _jwtService.CreateTokens(user, roles);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenResponse.AccessToken);

            // Assert
            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "user1");
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.GivenName && c.Value == "John");
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Surname && c.Value == "Doe");
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            Assert.Contains(token.Claims, c => c.Type == "Discount" && c.Value == "10");
            Assert.Contains(token.Claims, c => c.Type == "PhotoUrl" && c.Value == "https://example.com/photo.jpg");
        }


    }


}
