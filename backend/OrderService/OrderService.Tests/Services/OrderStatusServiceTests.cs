using AutoMapper;
using Moq;
using OrderService.BLL.DTOs.OrderStatusDTOs;
using OrderService.BLL.Exceptions;
using OrderService.BLL.Services;
using OrderService.BLL.Services.Interfaces;
using OrderService.DAL.UOW;
using OrderService.Domain.Entities;
using shared.cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrderService.Tests.Services
{
    public class OrderStatusServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IEntityCacheService> _cacheServiceMock;
        private readonly Mock<IEntityCacheInvalidationService<OrderStatus>> _cacheInvalidationMock;
        private readonly IOrderStatusService _sut;

        public OrderStatusServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _cacheServiceMock = new Mock<IEntityCacheService>();
            _cacheInvalidationMock = new Mock<IEntityCacheInvalidationService<OrderStatus>>();

            _sut = new OrderStatusService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _cacheServiceMock.Object,
                _cacheInvalidationMock.Object
            );
        }


        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllStatuses()
        {
            var statuses = new List<OrderStatus>
            {
                new OrderStatus { Id = Guid.NewGuid(), Name = "Pending" },
                new OrderStatus { Id = Guid.NewGuid(), Name = "Completed" }
            };

            _unitOfWorkMock.Setup(u => u.OrderStatuses.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(statuses);
            _mapperMock.Setup(m => m.Map<IEnumerable<OrderStatusReadDto>>(statuses))
                .Returns(new List<OrderStatusReadDto>
                {
                    new() { Id = statuses[0].Id, Name = "Pending" },
                    new() { Id = statuses[1].Id, Name = "Completed" }
                });

            var result = await _sut.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WhenStatusExists_ReturnsStatus()
        {
            var id = Guid.NewGuid();
            var status = new OrderStatus { Id = id, Name = "Pending" };
            var dto = new OrderStatusReadDto { Id = id, Name = "Pending" };

            _unitOfWorkMock.Setup(u => u.OrderStatuses.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(status);
            _mapperMock.Setup(m => m.Map<OrderStatusReadDto>(status)).Returns(dto);

            var result = await _sut.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Pending", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenStatusDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.OrderStatuses.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((OrderStatus)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(id));
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WhenNameIsUnique_CreatesStatus()
        {
            var dto = new OrderStatusCreateDto { Name = "NewStatus" };
            var entity = new OrderStatus { Id = Guid.NewGuid(), Name = dto.Name };
            var readDto = new OrderStatusReadDto { Id = entity.Id, Name = entity.Name };

            _unitOfWorkMock.Setup(u => u.OrderStatuses.IsNameDuplicatedAsync(dto.Name, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mapperMock.Setup(m => m.Map<OrderStatus>(dto)).Returns(entity);
            _mapperMock.Setup(m => m.Map<OrderStatusReadDto>(entity)).Returns(readDto);

            var result = await _sut.CreateAsync(dto);

            _unitOfWorkMock.Verify(u => u.OrderStatuses.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(dto.Name, result.Name);
        }

        [Fact]
        public async Task CreateAsync_WhenNameIsDuplicate_ThrowsAlreadyExistsException()
        {
            var dto = new OrderStatusCreateDto { Name = "DuplicateStatus" };

            _unitOfWorkMock.Setup(u => u.OrderStatuses.IsNameDuplicatedAsync(dto.Name, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AlreadyExistsException>(() => _sut.CreateAsync(dto));
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WhenStatusExists_UpdatesStatus()
        {
            var id = Guid.NewGuid();
            var dto = new OrderStatusUpdateDto { Name = "UpdatedStatus" };
            var status = new OrderStatus { Id = id, Name = "OldStatus" };
            var readDto = new OrderStatusReadDto { Id = id, Name = dto.Name };

            _unitOfWorkMock.Setup(u => u.OrderStatuses.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(status);
            _unitOfWorkMock.Setup(u => u.OrderStatuses.IsNameDuplicatedAsync(dto.Name, id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map(dto, status));
            _mapperMock.Setup(m => m.Map<OrderStatusReadDto>(status)).Returns(readDto);

            var result = await _sut.UpdateAsync(id, dto);

            _unitOfWorkMock.Verify(u => u.OrderStatuses.Update(status), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(dto.Name, result.Name);
        }

        [Fact]
        public async Task UpdateAsync_WhenStatusDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            var dto = new OrderStatusUpdateDto { Name = "UpdatedStatus" };
            _unitOfWorkMock.Setup(u => u.OrderStatuses.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((OrderStatus)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(id, dto));
        }

        [Fact]
        public async Task UpdateAsync_WhenNameIsDuplicate_ThrowsAlreadyExistsException()
        {
            var id = Guid.NewGuid();
            var dto = new OrderStatusUpdateDto { Name = "DuplicateStatus" };
            var status = new OrderStatus { Id = id, Name = "OldStatus" };

            _unitOfWorkMock.Setup(u => u.OrderStatuses.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(status);
            _unitOfWorkMock.Setup(u => u.OrderStatuses.IsNameDuplicatedAsync(dto.Name, id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<AlreadyExistsException>(() => _sut.UpdateAsync(id, dto));
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WhenStatusExists_DeletesStatus()
        {
            var id = Guid.NewGuid();
            var status = new OrderStatus { Id = id, Name = "ToDelete" };

            _unitOfWorkMock.Setup(u => u.OrderStatuses.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(status);

            await _sut.DeleteAsync(id);

            _unitOfWorkMock.Verify(u => u.OrderStatuses.Delete(status), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenStatusDoesNotExist_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.OrderStatuses.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((OrderStatus)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(id));
        }

        #endregion
    }
}
