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
    public class SizeServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ISizeRepository> _sizeRepoMock;
        private readonly SizeService _sut;

        public SizeServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _sizeRepoMock = new Mock<ISizeRepository>();

            _uowMock.Setup(u => u.Sizes).Returns(_sizeRepoMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedSizes()
        {
            var sizes = new List<Size>
            {
                new Size { Id = Guid.NewGuid(), Name = "Small" },
                new Size { Id = Guid.NewGuid(), Name = "Large" }
            };

            _sizeRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(sizes);
            _mapperMock.Setup(m => m.Map<IEnumerable<SizeDto>>(sizes))
                       .Returns(sizes.Select(s => new SizeDto(s.Id, s.Name)));

            var result = await _sut.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.Name == "Small");
            Assert.Contains(result, s => s.Name == "Large");
        }

        [Fact]
        public async Task GetByIdAsync_WhenSizeExists_ReturnsSize()
        {
            var id = Guid.NewGuid();
            var size = new Size { Id = id, Name = "Medium" };
            var sizeDto = new SizeDto(id, "Medium");

            _sizeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(size);
            _mapperMock.Setup(m => m.Map<SizeDto>(size)).Returns(sizeDto);

            var result = await _sut.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Medium", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenSizeDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _sizeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Size)null);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(id));
            Assert.Contains(id.ToString(), ex.Message);
        }

        [Fact]
        public async Task CreateAsync_WithValidName_CreatesSize()
        {
            var name = "ExtraLarge";
            _sizeRepoMock.Setup(r => r.ExistsWithNameAsync(name, null)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<SizeDto>(It.IsAny<Size>()))
                       .Returns((Size s) => new SizeDto(s.Id, s.Name));

            var result = await _sut.CreateAsync(name);

            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
            _sizeRepoMock.Verify(r => r.AddAsync(It.IsAny<Size>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenNameExists_ThrowsAlreadyExistsException()
        {
            var name = "Small";
            _sizeRepoMock.Setup(r => r.ExistsWithNameAsync(name, null)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<AlreadyExistsException>(() => _sut.CreateAsync(name));
            Assert.Contains(name, ex.Message);
            _sizeRepoMock.Verify(r => r.AddAsync(It.IsAny<Size>()), Times.Never);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenSizeExists_UpdatesSize()
        {
            var id = Guid.NewGuid();
            var size = new Size { Id = id, Name = "Medium" };
            var newName = "MediumPlus";

            _sizeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(size);
            _sizeRepoMock.Setup(r => r.ExistsWithNameAsync(newName, id)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<SizeDto>(size)).Returns(new SizeDto(id, newName));

            var result = await _sut.UpdateAsync(id, newName);

            Assert.Equal(newName, result.Name);
            _sizeRepoMock.Verify(r => r.Update(size), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenSizeDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _sizeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Size)null);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(id, "AnyName"));
            Assert.Contains(id.ToString(), ex.Message);
        }

        [Fact]
        public async Task UpdateAsync_WhenNameExists_ThrowsAlreadyExistsException()
        {
            var id = Guid.NewGuid();
            var size = new Size { Id = id, Name = "Medium" };
            _sizeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(size);
            _sizeRepoMock.Setup(r => r.ExistsWithNameAsync("Large", id)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<AlreadyExistsException>(() => _sut.UpdateAsync(id, "Large"));
            Assert.Contains("Large", ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_WhenSizeExists_DeletesSize()
        {
            var id = Guid.NewGuid();
            var size = new Size { Id = id, Name = "Medium" };
            _sizeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(size);

            await _sut.DeleteAsync(id);

            _sizeRepoMock.Verify(r => r.Delete(size), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenSizeDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _sizeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Size)null);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(id));
            Assert.Contains(id.ToString(), ex.Message);
        }
    }
}
