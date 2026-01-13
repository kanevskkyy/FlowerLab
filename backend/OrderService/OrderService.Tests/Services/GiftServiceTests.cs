using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using OrderService.BLL.DTOs.GiftsDTOs;
using OrderService.BLL.Exceptions;
using OrderService.BLL.Helpers;
using OrderService.BLL.Services;
using OrderService.BLL.Services.Interfaces;
using OrderService.DAL.Repositories.Interfaces;
using OrderService.DAL.UOW;
using OrderService.Domain.Entities;
using shared.cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OrderService.Tests.Services
{
    public class GiftServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IImageService> _imageServiceMock;
        private Mock<IGiftRepository> _giftRepositoryMock;
        private IGiftService _sut;

        private Mock<IEntityCacheService> _cacheServiceMock;
        private Mock<IEntityCacheInvalidationService<Gift>> _cacheInvalidationMock;


        public GiftServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _imageServiceMock = new Mock<IImageService>();
            _giftRepositoryMock = new Mock<IGiftRepository>();
            _cacheServiceMock = new Mock<IEntityCacheService>();
            _cacheInvalidationMock = new Mock<IEntityCacheInvalidationService<Gift>>();

            _unitOfWorkMock.Setup(u => u.Gifts).Returns(_giftRepositoryMock.Object);

            _sut = new GiftService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _imageServiceMock.Object,
                _cacheServiceMock.Object,
                _cacheInvalidationMock.Object
            );
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WhenGiftsExist_ReturnsAllGifts()
        {
            var gifts = new List<Gift>
            {
                new Gift { Id = Guid.NewGuid(), Name = "Gift 1" },
                new Gift { Id = Guid.NewGuid(), Name = "Gift 2" },
                new Gift { Id = Guid.NewGuid(), Name = "Gift 3" }
            };

            var giftDtos = gifts.Select(g => new GiftReadDto { Id = g.Id, Name = g.Name });

            _giftRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(gifts);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<GiftReadDto>>(It.IsAny<IEnumerable<Gift>>()))
                .Returns(giftDtos);

            var result = await _sut.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            _giftRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoGiftsExist_ReturnsEmptyCollection()
        {
            var emptyGifts = new List<Gift>();
            var emptyDtos = new List<GiftReadDto>();

            _giftRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyGifts);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<GiftReadDto>>(It.IsAny<IEnumerable<Gift>>()))
                .Returns(emptyDtos);

            var result = await _sut.GetAllAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
            _giftRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WhenGiftExists_ReturnsGift()
        {
            var giftId = Guid.NewGuid();
            var gift = new Gift { Id = giftId, Name = "Test Gift" };
            var giftDto = new GiftReadDto { Id = giftId, Name = "Test Gift" };

            _giftRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(gift);

            _mapperMock
                .Setup(m => m.Map<GiftReadDto>(It.IsAny<Gift>()))
                .Returns(giftDto);

            var result = await _sut.GetByIdAsync(giftId);

            Assert.NotNull(result);
            Assert.Equal(giftId, result.Id);
            Assert.Equal("Test Gift", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenGiftDoesNotExist_ThrowsNotFoundException()
        {
            var giftId = Guid.NewGuid();

            _giftRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Gift)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.GetByIdAsync(giftId)
            );

            Assert.Contains(giftId.ToString(), exception.Message);
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesAndReturnsGift()
        {
            // Arrange
            var content = new byte[] { 1, 2, 3 };
            var stream = new MemoryStream(content);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns("image.jpg");
            fileMock.Setup(f => f.Length).Returns(content.Length);

            var createDto = new GiftCreateDto
            {
                Name = "New Gift",
                Image = fileMock.Object
            };

            var imageUrl = "https://storage.com/gifts/image.jpg";
            var giftReadDto = new GiftReadDto
            {
                Id = Guid.NewGuid(),
                Name = "New Gift",
                ImageUrl = imageUrl
            };

            Gift capturedGift = null;

            _giftRepositoryMock.Setup(r => r.IsNameDuplicatedAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

            _imageServiceMock.Setup(i => i.UploadAsync(It.IsAny<IFormFile>(), "order-service/gifts"))
                .ReturnsAsync(imageUrl);

            _mapperMock.Setup(m => m.Map<Gift>(It.IsAny<GiftCreateDto>()))
                .Returns(new Gift());

            _giftRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Gift>(), It.IsAny<CancellationToken>()))
                .Callback<Gift, CancellationToken>((g, ct) => capturedGift = g)
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<GiftReadDto>(It.IsAny<Gift>()))
                .Returns(giftReadDto);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Gift", result.Name);
            Assert.Equal(imageUrl, result.ImageUrl);

            _giftRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Gift>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _imageServiceMock.Verify(i => i.UploadAsync(It.IsAny<IFormFile>(), "order-service/gifts"), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenNameIsDuplicated_ThrowsAlreadyExistsException()
        {
            // Arrange
            var content = new byte[] { 1, 2, 3 };
            var stream = new MemoryStream(content);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns("image.jpg");
            fileMock.Setup(f => f.Length).Returns(content.Length);

            var createDto = new GiftCreateDto
            {
                Name = "Duplicate Gift",
                Image = fileMock.Object
            };

            _giftRepositoryMock.Setup(r => r.IsNameDuplicatedAsync(
                    It.IsAny<string>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<AlreadyExistsException>(
                () => _sut.CreateAsync(createDto)
            );

            Assert.Contains(createDto.Name, exception.Message);

            _imageServiceMock.Verify(i => i.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
            _giftRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Gift>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesAndReturnsGift()
        {
            var giftId = Guid.NewGuid();
            var existingGift = new Gift
            {
                Id = giftId,
                Name = "Old Name",
                ImageUrl = "old-url"
            };

            var updateDto = new GiftUpdateDto
            {
                Name = "Updated Name"
            };

            var updatedGiftDto = new GiftReadDto
            {
                Id = giftId,
                Name = "Updated Name"
            };

            _giftRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingGift);

            _giftRepositoryMock.Setup(r => r.IsNameDuplicatedAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mapperMock.Setup(m => m.Map(It.IsAny<GiftUpdateDto>(), It.IsAny<Gift>()))
                .Returns((GiftUpdateDto dto, Gift g) =>
                {
                    g.Name = dto.Name;
                    return g;
                });

            _mapperMock.Setup(m => m.Map<GiftReadDto>(It.IsAny<Gift>()))
                .Returns(updatedGiftDto);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateAsync(giftId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            _giftRepositoryMock.Verify(r => r.Update(It.IsAny<Gift>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenGiftDoesNotExist_ThrowsNotFoundException()
        {
            var giftId = Guid.NewGuid();
            var updateDto = new GiftUpdateDto { Name = "Test" };

            _giftRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Gift)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.UpdateAsync(giftId, updateDto)
            );

            Assert.Contains(giftId.ToString(), exception.Message);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WhenGiftExists_DeletesGiftAndImage()
        {
            var giftId = Guid.NewGuid();
            var existingGift = new Gift { Id = giftId, Name = "Gift", ImageUrl = "image-url" };

            _giftRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingGift);
            _imageServiceMock.Setup(i => i.DeleteImageAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            await _sut.DeleteAsync(giftId);

            _imageServiceMock.Verify(i => i.DeleteImageAsync("image-url"), Times.Once);
            _giftRepositoryMock.Verify(r => r.Delete(It.IsAny<Gift>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenGiftDoesNotExist_ThrowsNotFoundException()
        {
            var giftId = Guid.NewGuid();

            _giftRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Gift)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.DeleteAsync(giftId)
            );

            Assert.Contains(giftId.ToString(), exception.Message);
        }

        #endregion
    }
}
