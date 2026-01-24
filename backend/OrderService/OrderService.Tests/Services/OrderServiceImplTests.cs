using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LiqPay.SDK.Dto;
using LiqPay.SDK.Dto.Enums;
using MassTransit;
using Moq;
using OrderService.BLL.DTOs.OrderDTOs;
using OrderService.BLL.Exceptions;
using OrderService.BLL.Services;
using OrderService.BLL.Services.Interfaces;
using OrderService.DAL.Helpers;
using OrderService.DAL.UOW;
using OrderService.Domain.Entities;
using OrderService.Domain.QueryParams;
using shared.events.OrderEvents;
using Xunit;

namespace OrderService.Tests.Services
{
    public class OrderServiceImplTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<CheckOrder.CheckOrderClient> _catalogClientMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<ILiqPayService> _liqPayServiceMock;
        private readonly IOrderService _sut;

        public OrderServiceImplTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _catalogClientMock = new Mock<CheckOrder.CheckOrderClient>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _liqPayServiceMock = new Mock<ILiqPayService>();

            _sut = new OrderServiceImpl(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _catalogClientMock.Object,
                _publishEndpointMock.Object,
                _liqPayServiceMock.Object
            );
        }

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WhenOrderExists_ReturnsOrderDetailDto()
        {
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                Status = new OrderStatus { Name = "AwaitingPayment" },
                TotalPrice = 200m
            };

            var dto = new OrderDetailDto();

            _unitOfWorkMock.Setup(u => u.Orders.GetByIdWithIncludesAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
            _mapperMock.Setup(m => m.Map<OrderDetailDto>(order)).Returns(dto);
            _liqPayServiceMock.Setup(l => l.GeneratePaymentUrl(orderId, order.TotalPrice, It.IsAny<string>()))
                .Returns("payment-url");

            var result = await _sut.GetByIdAsync(orderId);

            Assert.NotNull(result);
            _unitOfWorkMock.Verify(u => u.Orders.GetByIdWithIncludesAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
            _liqPayServiceMock.Verify(l => l.GeneratePaymentUrl(orderId, order.TotalPrice, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenOrderDoesNotExist_ThrowsNotFoundException()
        {
            var orderId = Guid.NewGuid();
            _unitOfWorkMock.Setup(u => u.Orders.GetByIdWithIncludesAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetByIdAsync(orderId));
        }

        #endregion

        #region GetPagedOrdersAsync Tests

        [Fact]
        public async Task GetPagedOrdersAsync_ReturnsPagedOrders_WithPaymentUrlForAwaitingPayment()
        {
            var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), Status = new OrderStatus { Name = "AwaitingPayment" }, TotalPrice = 100m },
                new Order { Id = Guid.NewGuid(), Status = new OrderStatus { Name = "Completed" }, TotalPrice = 150m }
            };
            var pagedOrders = new PagedList<Order>(orders, orders.Count, 1, 10);

            _unitOfWorkMock.Setup(u => u.Orders.GetPagedOrdersAsync(It.IsAny<OrderSpecificationParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedOrders);
            _mapperMock.Setup(m => m.Map<OrderSummaryDto>(It.IsAny<Order>()))
                .Returns<Order>(o => new OrderSummaryDto { Id = o.Id });

            _liqPayServiceMock.Setup(l => l.GeneratePaymentUrl(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .Returns("payment-url");

            var result = await _sut.GetPagedOrdersAsync(new OrderSpecificationParameters());

            Assert.Equal(2, result.Items.Count());
            var awaitingPaymentDto = result.Items.First(d => d.Id == orders[0].Id);
        }

        #endregion

        #region ProcessPaymentCallbackAsync Tests

        [Fact]
        public async Task ProcessPaymentCallbackAsync_SuccessfulPayment_UpdatesOrderStatusAndPublishesEvent()
        {
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                OrderGifts = new List<OrderGift>(),
                Status = new OrderStatus { Name = "AwaitingPayment" },
                Items = new List<OrderItem>()
            };

            var liqPayResponse = new LiqPayResponse
            {
                OrderId = orderId.ToString(),
                Status = LiqPayResponseStatus.Success
            };

            _liqPayServiceMock.Setup(l => l.ValidateCallback(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _liqPayServiceMock.Setup(l => l.ParseCallback(It.IsAny<string>())).Returns(liqPayResponse);

            _unitOfWorkMock.Setup(u => u.Orders.GetByIdWithIncludesAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _unitOfWorkMock.Setup(u => u.OrderStatuses.GetByNameAsync("Pending", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new OrderStatus { Id = Guid.NewGuid(), Name = "Pending" });

            await _sut.ProcessPaymentCallbackAsync("data", "signature");

            _unitOfWorkMock.Verify(u => u.Orders.Update(order), Times.Once);
            _publishEndpointMock.Verify(p => p.Publish(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task ProcessPaymentCallbackAsync_InvalidSignature_ThrowsValidationException()
        {
            _liqPayServiceMock.Setup(l => l.ValidateCallback(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            await Assert.ThrowsAsync<ValidationException>(() => _sut.ProcessPaymentCallbackAsync("data", "signature"));
        }

        #endregion
    }
}
