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

namespace CatalogService.Tests.Services
{
    public class FlowerServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IFlowerRepository> _flowerRepoMock;
        private readonly FlowerService _sut;

        public FlowerServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _flowerRepoMock = new Mock<IFlowerRepository>();

            _uowMock.Setup(u => u.Flowers).Returns(_flowerRepoMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedFlowers()
        {
            var flowers = new List<Flower>
            {
                new Flower { Id = Guid.NewGuid(), Name = "Rose" },
                new Flower { Id = Guid.NewGuid(), Name = "Tulip" }
            };

            _flowerRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(flowers);
            _mapperMock.Setup(m => m.Map<IEnumerable<FlowerDto>>(flowers))
                       .Returns(flowers.Select(f => new FlowerDto { Id = f.Id, Name = f.Name }));

            var result = await _sut.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, f => f.Name == "Rose");
            Assert.Contains(result, f => f.Name == "Tulip");
        }

        [Fact]
        public async Task GetByIdAsync_WhenFlowerExists_ReturnsFlower()
        {
            var flowerId = Guid.NewGuid();
            var flower = new Flower { Id = flowerId, Name = "Rose" };
            var flowerDto = new FlowerDto { Id = flowerId, Name = "Rose" };

            _flowerRepoMock.Setup(r => r.GetByIdAsync(flowerId)).ReturnsAsync(flower);
            _mapperMock.Setup(m => m.Map<FlowerDto>(flower)).Returns(flowerDto);

            var result = await _sut.GetByIdAsync(flowerId);

            Assert.NotNull(result);
            Assert.Equal(flowerId, result.Id);
            Assert.Equal("Rose", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenFlowerDoesNotExist_ThrowsNotFoundException()
        {
            var flowerId = Guid.NewGuid();
            _flowerRepoMock.Setup(r => r.GetByIdAsync(flowerId)).ReturnsAsync((Flower)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(flowerId));

            Assert.Contains(flowerId.ToString(), exception.Message);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesFlower()
        {
            var dto = new FlowerCreateUpdateDto
            {
                Name = "Lily",
                Color = "White",
                Quantity = 5
            };

            _flowerRepoMock.Setup(r => r.ExistsWithNameAsync(dto.Name, null)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<FlowerDto>(It.IsAny<Flower>()))
                       .Returns((Flower f) => new FlowerDto { Id = f.Id, Name = f.Name });

            var result = await _sut.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Lily", result.Name);

            _flowerRepoMock.Verify(r => r.AddAsync(It.IsAny<Flower>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenNameExists_ThrowsAlreadyExistsException()
        {
            var dto = new FlowerCreateUpdateDto { Name = "Lily" };
            _flowerRepoMock.Setup(r => r.ExistsWithNameAsync(dto.Name, null)).ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<AlreadyExistsException>(() => _sut.CreateAsync(dto));

            Assert.Contains("Lily", exception.Message);
            _flowerRepoMock.Verify(r => r.AddAsync(It.IsAny<Flower>()), Times.Never);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenFlowerExists_UpdatesFlower()
        {
            var flowerId = Guid.NewGuid();
            var existingFlower = new Flower { Id = flowerId, Name = "Rose", Quantity = 10 };
            var dto = new FlowerCreateUpdateDto { Name = "Tulip", Quantity = 20 };

            _flowerRepoMock.Setup(r => r.GetByIdAsync(flowerId)).ReturnsAsync(existingFlower);
            _flowerRepoMock.Setup(r => r.ExistsWithNameAsync(dto.Name, flowerId)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<FlowerDto>(existingFlower))
                       .Returns(new FlowerDto { Id = flowerId, Name = dto.Name });

            var result = await _sut.UpdateAsync(flowerId, dto);

            Assert.Equal("Tulip", result.Name);
            _flowerRepoMock.Verify(r => r.Update(existingFlower), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenFlowerDoesNotExist_ThrowsNotFoundException()
        {
            var flowerId = Guid.NewGuid();
            _flowerRepoMock.Setup(r => r.GetByIdAsync(flowerId)).ReturnsAsync((Flower)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(flowerId, new FlowerCreateUpdateDto()));

            Assert.Contains(flowerId.ToString(), exception.Message);
        }

        [Fact]
        public async Task DeleteAsync_WhenFlowerExists_DeletesFlower()
        {
            var flowerId = Guid.NewGuid();
            var flower = new Flower { Id = flowerId, Name = "Rose" };
            _flowerRepoMock.Setup(r => r.GetByIdAsync(flowerId)).ReturnsAsync(flower);

            await _sut.DeleteAsync(flowerId);

            _flowerRepoMock.Verify(r => r.Delete(flower), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenFlowerDoesNotExist_ThrowsNotFoundException()
        {
            var flowerId = Guid.NewGuid();
            _flowerRepoMock.Setup(r => r.GetByIdAsync(flowerId)).ReturnsAsync((Flower)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(flowerId));

            Assert.Contains(flowerId.ToString(), exception.Message);
        }
    }
}
