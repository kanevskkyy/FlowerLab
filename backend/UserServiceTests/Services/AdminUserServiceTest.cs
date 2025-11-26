using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.Models;
using UsersService.BLL.Services;
using UsersService.Domain.Entities;
using Xunit;

namespace UsersService.Tests.Services
{
    public class AdminUserServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AdminUserService _service;

        public AdminUserServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _mapperMock = new Mock<IMapper>();

            _service = new AdminUserService(_userManagerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUserDto_WhenUserExists()
        {
            // Arrange
            var userId = "123";
            var user = new ApplicationUser { Id = userId, FirstName = "John", LastName = "Doe" };
            var userDto = new AdminUserDto { Id = userId, FirstName = "John", LastName = "Doe" };

            _userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _userManagerMock.Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin" });

            _mapperMock.Setup(m => m.Map<AdminUserDto>(user))
                .Returns(userDto);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result!.FirstName);
            Assert.Equal("Admin", result.Role);
        }

       

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            _userManagerMock.Setup(u => u.FindByIdAsync("99")).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _service.GetUserByIdAsync("99");

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(50, true)]
        [InlineData(-1, false)]
        [InlineData(101, false)]
        public async Task UpdateUserDiscountAsync_ValidatesDiscount(int discount, bool expectedResult)
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", PersonalDiscountPercentage = 0 };
            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.UpdateUserDiscountAsync("1", discount);

            // Assert
            Assert.Equal(expectedResult, result);
            if (expectedResult) Assert.Equal(discount, user.PersonalDiscountPercentage);
        }

        [Fact]
        public async Task UpdateUserDiscountAsync_ReturnsFalse_WhenUserNotFound()
        {
            // Arrange
            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _service.UpdateUserDiscountAsync("1", 10);

            // Assert
            Assert.False(result);
        }
    }
}
