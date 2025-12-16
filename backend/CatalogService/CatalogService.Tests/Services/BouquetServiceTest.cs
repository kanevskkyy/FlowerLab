using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using MassTransit;

using CatalogService.BLL.Services.Interfaces;
using CatalogService.BLL.Services.Implementations;
using AutoMapper;
using CatalogService.Domain.Entities;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Exceptions;
using CatalogService.DAL.Repositories.Interfaces; 
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.QueryParametrs;
using CatalogService.DAL.Helpers;
using FlowerLab.Shared.Events;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace CatalogService.Tests.Services
{
    public class BouquetServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly BouquetService _sut;

        public BouquetServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _imageServiceMock = new Mock<IImageService>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
        }

        private static IFormFile CreateFakeFile(string fileName = "test.jpg", string content = "fake image")
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");

            return fileMock.Object;
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_ReturnsBouquetDto()
        {
            var sizeId = Guid.NewGuid();
            var flowerId = Guid.NewGuid();
            var createDto = new BouquetCreateDto
            {
                Name = "Test Bouquet",
                Sizes = new List<BouquetSizeCreateDto>
                {
                    new BouquetSizeCreateDto
                    {
                        SizeId = sizeId,
                        Price = 100,
                        FlowerIds = new List<Guid> { flowerId },
                        FlowerQuantities = new List<int> { 1 }
                    }
                },
                MainPhoto = CreateFakeFile(),
                Images = new List<IFormFile> { CreateFakeFile("img1.jpg") }
            };

            _uowMock.Setup(u => u.Bouquets.ExistsAsync(It.IsAny<Expression<Func<Bouquet, bool>>>()))
    .ReturnsAsync(false);
            _uowMock.Setup(u => u.Sizes.GetByIdAsync(sizeId))
                .ReturnsAsync(new Size { Id = sizeId, Name = "Medium" });
            _uowMock.Setup(u => u.Flowers.GetByIdAsync(flowerId))
                .ReturnsAsync(new Flower { Id = flowerId, Name = "Rose", Quantity = 10 });
            _imageServiceMock.Setup(i => i.UploadAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("http://fakeurl.com/image.jpg");
            _uowMock.Setup(u => u.Bouquets.AddAsync(It.IsAny<Bouquet>()))
                .Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _uowMock.Setup(u => u.Bouquets.GetWithDetailsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Bouquet { Id = Guid.NewGuid(), Name = createDto.Name, MainPhotoUrl = "http://fakeurl.com/image.jpg" });

            _mapperMock.Setup(m => m.Map<BouquetDto>(It.IsAny<Bouquet>()))
                .Returns((Bouquet b) => new BouquetDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    MainPhotoUrl = b.MainPhotoUrl
                });

            var result = await _sut.CreateAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal(createDto.Name, result.Name);
            Assert.Equal("http://fakeurl.com/image.jpg", result.MainPhotoUrl);
        }

        [Fact]
        public async Task CreateAsync_WhenNameExists_ThrowsAlreadyExistsException()
        {
            var createDto = new BouquetCreateDto { Name = "Duplicate Bouquet" };
            _uowMock.Setup(u => u.Bouquets.ExistsAsync(It.IsAny<Expression<Func<Bouquet, bool>>>()))
    .ReturnsAsync(true);

            await Assert.ThrowsAsync<AlreadyExistsException>(() => _sut.CreateAsync(createDto));
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WhenBouquetExists_ReturnsBouquet()
        {
            var id = Guid.NewGuid();
            _uowMock.Setup(u => u.Bouquets.GetWithDetailsAsync(id))
                .ReturnsAsync(new Bouquet { Id = id, Name = "Test Bouquet" });
            _mapperMock.Setup(m => m.Map<BouquetDto>(It.IsAny<Bouquet>()))
                .Returns((Bouquet b) => new BouquetDto { Id = b.Id, Name = b.Name });

            var result = await _sut.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Test Bouquet", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenBouquetDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _uowMock.Setup(u => u.Bouquets.GetWithDetailsAsync(id)).ReturnsAsync((Bouquet)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(id));
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WhenBouquetExists_UpdatesBouquet()
        {
            var id = Guid.NewGuid();
            var sizeId = Guid.NewGuid();
            var flowerId = Guid.NewGuid();

            var existingBouquet = new Bouquet
            {
                Id = id,
                Name = "Old Name",
                BouquetSizes = new List<BouquetSize>()
            };

            var updateDto = new BouquetUpdateDto
            {
                Name = "New Name",
                Sizes = new List<BouquetSizeCreateDto>
                {
                    new BouquetSizeCreateDto
                    {
                        SizeId = sizeId,
                        Price = 150,
                        FlowerIds = new List<Guid> { flowerId },
                        FlowerQuantities = new List<int> { 2 }
                    }
                }
            };

            _uowMock.Setup(u => u.Bouquets.GetWithDetailsAsync(id)).ReturnsAsync(existingBouquet);
            _uowMock.Setup(u => u.Bouquets.ExistsAsync(It.IsAny<Expression<Func<Bouquet, bool>>>()))
                .ReturnsAsync(false);
            _uowMock.Setup(u => u.Sizes.GetByIdAsync(sizeId)).ReturnsAsync(new Size { Id = sizeId, Name = "Medium" });
            _uowMock.Setup(u => u.Flowers.GetByIdAsync(flowerId)).ReturnsAsync(new Flower { Id = flowerId, Name = "Rose", Quantity = 10 });
            _imageServiceMock.Setup(i => i.UploadAsync(It.IsAny<byte[]>(), It.IsAny<string>(), "bouquets"))
                .ReturnsAsync("http://fakeurl.com/image.jpg");
            _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<BouquetDto>(It.IsAny<Bouquet>()))
                .Returns((Bouquet b) => new BouquetDto { Id = b.Id, Name = b.Name });

            var result = await _sut.UpdateAsync(id, updateDto);

            Assert.NotNull(result);
            Assert.Equal("New Name", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_WhenBouquetDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _uowMock.Setup(u => u.Bouquets.GetWithDetailsAsync(id)).ReturnsAsync((Bouquet)null);
            var updateDto = new BouquetUpdateDto { Name = "Test" };

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(id, updateDto));
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WhenBouquetExists_DeletesBouquet()
        {
            var id = Guid.NewGuid();
            var bouquet = new Bouquet { Id = id, Name = "Test Bouquet" };
            _uowMock.Setup(u => u.Bouquets.GetWithDetailsAsync(id)).ReturnsAsync(bouquet);
            _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _publishEndpointMock.Setup(p => p.Publish(It.IsAny<BouquetDeletedEvent>(), default)).Returns(Task.CompletedTask);

            await _sut.DeleteAsync(id);

            _uowMock.Verify(u => u.Bouquets.Delete(bouquet), Times.Once);
            _publishEndpointMock.Verify(p => p.Publish(It.IsAny<BouquetDeletedEvent>(), default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenBouquetDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _uowMock.Setup(u => u.Bouquets.GetWithDetailsAsync(id)).ReturnsAsync((Bouquet)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(id));
        }

        #endregion

        #region UpdateFlowerQuantityAsync Tests

        [Fact]
        public async Task UpdateFlowerQuantityAsync_WhenFlowerExists_UpdatesQuantity()
        {
            var flowerId = Guid.NewGuid();
            var flower = new Flower { Id = flowerId, Quantity = 5 };

            _uowMock.Setup(u => u.Flowers.GetByIdAsync(flowerId)).ReturnsAsync(flower);
            _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            await _sut.UpdateFlowerQuantityAsync(flowerId, 10);

            Assert.Equal(10, flower.Quantity);
        }

        [Fact]
        public async Task UpdateFlowerQuantityAsync_WhenFlowerDoesNotExist_ThrowsNotFoundException()
        {
            var flowerId = Guid.NewGuid();
            _uowMock.Setup(u => u.Flowers.GetByIdAsync(flowerId)).ReturnsAsync((Flower)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateFlowerQuantityAsync(flowerId, 10));
        }

        [Fact]
        public async Task UpdateFlowerQuantityAsync_WhenQuantityNegative_ThrowsArgumentException()
        {
            var flowerId = Guid.NewGuid();
            var flower = new Flower { Id = flowerId, Quantity = 5 };
            _uowMock.Setup(u => u.Flowers.GetByIdAsync(flowerId)).ReturnsAsync(flower);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateFlowerQuantityAsync(flowerId, -1));
        }

        #endregion
    }
}
