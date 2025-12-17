using AutoMapper;
using Grpc.Core;
using LiqPay.SDK.Dto.Enums;
using MassTransit;
using OrderService.BLL.DTOs.OrderDTOs;
using OrderService.BLL.Exceptions;
using OrderService.BLL.Services.Interfaces;
using OrderService.DAL.Helpers;
using OrderService.DAL.UOW;
using OrderService.Domain.Entities;
using OrderService.Domain.QueryParams;
using shared.events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.BLL.Services
{
    public class OrderServiceImpl : IOrderService
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;
        private CheckOrder.CheckOrderClient catalogClient;
        private IPublishEndpoint publishEndpoint;
        private ILiqPayService liqPayService;

        public OrderServiceImpl(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CheckOrder.CheckOrderClient catalogClient,
            IPublishEndpoint publishEndpoint,
            ILiqPayService liqPayService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.catalogClient = catalogClient;
            this.publishEndpoint = publishEndpoint;
            this.liqPayService = liqPayService;
        }

        public async Task<PagedList<OrderSummaryDto>> GetPagedOrdersAsync(OrderSpecificationParameters parameters, CancellationToken cancellationToken = default)
        {
            var pagedOrders = await unitOfWork.Orders.GetPagedOrdersAsync(parameters, cancellationToken);
            var dtoList = pagedOrders.Items.Select(o =>
            {
                var dto = mapper.Map<OrderSummaryDto>(o);
                if (o.Status?.Name == "AwaitingPayment")
                {
                    dto.PaymentUrl = liqPayService.GeneratePaymentUrl(
                        o.Id,
                        o.TotalPrice,
                        $"Оплата замовлення #{o.Id}");
                }

                return dto;
            }).ToList();

            return new PagedList<OrderSummaryDto>(dtoList, pagedOrders.TotalCount, pagedOrders.CurrentPage, pagedOrders.PageSize);
        }

        public async Task<OrderDetailDto> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await unitOfWork.Orders.GetByIdWithIncludesAsync(orderId, cancellationToken);
            if (order == null)
                throw new NotFoundException($"Замовлення з ID {orderId} не знайдено");

            var resultDto = mapper.Map<OrderDetailDto>(order);

            if (order.Status?.Name == "AwaitingPayment")
            {
                resultDto.PaymentUrl = liqPayService.GeneratePaymentUrl(
                    order.Id,
                    order.TotalPrice,
                    $"Оплата замовлення #{order.Id}");
            }

            return resultDto;
        }

        public async Task<OrderDetailDto> CreateAsync(
            Guid? userId,
            string? userFirstName,
            string? userLastName,
            string? userPhoneNumber,
            OrderCreateDto dto,
            decimal personalDiscount,
            CancellationToken cancellationToken = default)
        {
            var finalFirstName = userFirstName ?? dto.FirstName;
            var finalLastName = userLastName ?? dto.LastName;
            var finalPhoneNumber = userPhoneNumber ?? dto.PhoneNumber;

            var grpcRequest = new OrderedBouquetsIdList();
            foreach (var item in dto.Items)
            {
                grpcRequest.OrderedBouquets.Add(new OrderedBouquetsId
                {
                    Id = item.BouquetId.ToString(),
                    SizeId = item.SizeId.ToString(), 
                    Count = item.Count
                });
            }

            OrderedResponseList catalogResponse;
            try
            {
                catalogResponse = await catalogClient.CheckOrderItemsAsync(
                    grpcRequest, cancellationToken: cancellationToken);
            }
            catch (RpcException ex)
            {
                throw new ValidationException($"Помилка перевірки букетів: {ex.Status.Detail}");
            }

            var invalidItems = catalogResponse.OrderedResponseList_
                .Where(r => !r.IsValid)
                .ToList();

            if (invalidItems.Any())
            {
                var errors = string.Join("; ", invalidItems.Select(i =>
                    $"Букет '{i.BouquetName}' (розмір {i.SizeName}): {i.ErrorMessage}"));
                throw new ValidationException($"Помилка перевірки букетів: {errors}");
            }

            var awaitingPaymentStatus = await unitOfWork.OrderStatuses.GetByNameAsync("AwaitingPayment");
            if (awaitingPaymentStatus == null)
            {
                awaitingPaymentStatus = new OrderStatus
                {
                    Name = "AwaitingPayment",
                    CreatedAt = DateTime.UtcNow.ToUniversalTime(),
                    UpdatedAt = DateTime.UtcNow.ToUniversalTime()
                };
                await unitOfWork.OrderStatuses.AddAsync(awaitingPaymentStatus);
            }

            var existingOrders = await unitOfWork.Orders.GetPagedOrdersAsync(
                new OrderSpecificationParameters { UserId = userId },
                cancellationToken);

            bool isFirstOrder = !existingOrders.Items.Any(o => o.Items.Any());

            if (dto.Gifts != null && dto.Gifts.GroupBy(g => g.GiftId).Any(g => g.Count() > 1))
                throw new ValidationException("У замовленні не дозволяється дублювати подарунки.");

            List<OrderGift> orderGifts = new();
            if (dto.Gifts != null)
            {
                foreach (var giftDto in dto.Gifts)
                {
                    var gift = await unitOfWork.Gifts.GetByIdAsync(giftDto.GiftId)
                        ?? throw new NotFoundException($"Подарунок з ID {giftDto.GiftId} не знайдено");

                    if (gift.AvailableCount < giftDto.Count)
                        throw new ValidationException($"Недостатньо подарунків '{gift.Name}'. " +
                            $"Запитано {giftDto.Count}, доступно {gift.AvailableCount}.");

                    orderGifts.Add(new OrderGift
                    {
                        GiftId = gift.Id,
                        Gift = gift,
                        Count = giftDto.Count
                    });
                }
            }

            var itemsList = dto.Items.ToList();
            List<OrderItem> orderItems = new();

            for (int i = 0; i < itemsList.Count; i++)
            {
                var itemDto = itemsList[i];
                var catalogItem = catalogResponse.OrderedResponseList_[i];

                if (!decimal.TryParse(catalogItem.Price, out decimal price))
                    throw new ValidationException($"Некоректна ціна для букета {catalogItem.BouquetName}");

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    BouquetId = itemDto.BouquetId,
                    SizeId = itemDto.SizeId,
                    BouquetName = catalogItem.BouquetName,
                    BouquetImage = catalogItem.BouquetImage,
                    SizeName = catalogItem.SizeName,
                    Price = price,
                    Count = itemDto.Count,
                    Flowers = new List<OrderItemFlower>()
                };

                foreach (var flowerInfo in catalogItem.Flowers)
                {
                    if (Guid.TryParse(flowerInfo.FlowerId, out Guid flowerId))
                    {
                        orderItem.Flowers.Add(new OrderItemFlower
                        {
                            OrderItemId = orderItem.Id,
                            FlowerId = flowerId,
                            FlowerName = flowerInfo.FlowerName,
                            FlowerColor = flowerInfo.FlowerColor,
                            Quantity = flowerInfo.Quantity
                        });
                    }
                }

                orderItems.Add(orderItem);
            }

            var order = new Order
            {
                UserId = userId,
                UserFirstName = finalFirstName,
                UserLastName = finalLastName,
                PickupStoreAddress = dto.PickupStoreAddress,
                Notes = dto.Notes,
                PhoneNumber = finalPhoneNumber,
                GiftMessage = dto.GiftMessage,
                StatusId = awaitingPaymentStatus.Id,
                IsDelivery = dto.IsDelivery,
                Items = orderItems,
                DeliveryInformation = dto.DeliveryInformation != null
                    ? mapper.Map<DeliveryInformation>(dto.DeliveryInformation)
                    : null,
                OrderGifts = orderGifts
            };

            decimal itemsTotal = order.Items.Sum(i => i.Price * i.Count);

            if (order.OrderGifts.Any())
            {
                foreach (var orderGift in order.OrderGifts)
                {
                    itemsTotal += orderGift.Gift.Price * orderGift.Count;
                }
            }

            if (!string.IsNullOrWhiteSpace(order.GiftMessage)) itemsTotal += 50m;
            if (isFirstOrder) itemsTotal *= 0.9m;
            if (personalDiscount > 0) itemsTotal *= 1 - personalDiscount;

            order.TotalPrice = itemsTotal;

            await unitOfWork.Orders.AddAsync(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var resultDto = mapper.Map<OrderDetailDto>(order);
            resultDto.PaymentUrl = liqPayService.GeneratePaymentUrl(
                order.Id,
                order.TotalPrice,
                $"Оплата замовлення #{order.Id}");
            resultDto.TotalPrice = order.TotalPrice;

            return resultDto;
        }

        public async Task ProcessPaymentCallbackAsync(string data, string signature, CancellationToken cancellationToken = default)
        {
            if (!liqPayService.ValidateCallback(data, signature))
                throw new ValidationException("Невалідний підпис LiqPay");

            var response = liqPayService.ParseCallback(data);
            var orderId = Guid.Parse(response.OrderId);

            var order = await unitOfWork.Orders.GetByIdWithIncludesAsync(orderId, cancellationToken)
                ?? throw new NotFoundException($"Замовлення {orderId} не знайдено");

            if (response.Status == LiqPayResponseStatus.Success || response.Status == LiqPayResponseStatus.Sandbox)
            {
                var paidStatus = await unitOfWork.OrderStatuses.GetByNameAsync("Pending")
                    ?? throw new NotFoundException("Статус 'Pending' не знайдено");

                order.StatusId = paidStatus.Id;
                order.UpdatedAt = DateTime.UtcNow.ToUniversalTime();

                foreach (var orderGift in order.OrderGifts)
                {
                    var gift = await unitOfWork.Gifts.GetByIdAsync(orderGift.GiftId);
                    if (gift != null)
                    {
                        if (gift.AvailableCount < orderGift.Count)
                            throw new ValidationException($"Недостатньо подарунків '{gift.Name}' для завершення замовлення.");

                        gift.AvailableCount -= orderGift.Count;
                        gift.UpdatedAt = DateTime.UtcNow.ToUniversalTime();
                        unitOfWork.Gifts.Update(gift);
                    }
                }

                unitOfWork.Orders.Update(order);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var orderCreatedEvent = new OrderCreatedEvent
                {
                    OrderId = order.Id,
                    Bouquets = order.Items.Select(i => new OrderBouquetItem
                    {
                        BouquetId = i.BouquetId,
                        Count = i.Count
                    }).ToList()
                };
                await publishEndpoint.Publish(orderCreatedEvent, cancellationToken);

                TelegramOrderCreatedEvent telegramOrderCreatedEvent = new TelegramOrderCreatedEvent
                {
                    CustomerName = $"{order.UserFirstName} {order.UserLastName}",
                    OrderId = order.Id,
                    TotalPrice = order.TotalPrice,
                };
                await publishEndpoint.Publish(telegramOrderCreatedEvent, cancellationToken);

                if (order.IsDelivery)
                {
                    OrderAddressEvent orderAddressEvent = new OrderAddressEvent()
                    {
                        Address = order.DeliveryInformation.Address,
                        UserId = order.UserId.ToString()
                    };
                    await publishEndpoint.Publish(orderAddressEvent, cancellationToken);
                }

            }
            else if (response.Status == LiqPayResponseStatus.Failure || response.Status == LiqPayResponseStatus.Error)
            {
                var failedStatus = await unitOfWork.OrderStatuses.GetByNameAsync("PaymentFailed");
                if (failedStatus == null)
                {
                    failedStatus = new OrderStatus
                    {
                        Name = "PaymentFailed",
                        CreatedAt = DateTime.UtcNow.ToUniversalTime(),
                        UpdatedAt = DateTime.UtcNow.ToUniversalTime()
                    };
                    await unitOfWork.OrderStatuses.AddAsync(failedStatus);
                }

                order.StatusId = failedStatus.Id;
                order.UpdatedAt = DateTime.UtcNow.ToUniversalTime();
                unitOfWork.Orders.Update(order);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<PagedList<OrderSummaryDto>> GetMyOrdersAsync(Guid userId, OrderSpecificationParameters parameters, CancellationToken cancellationToken = default)
        {
            parameters.UserId = userId;
            var pagedOrders = await unitOfWork.Orders.GetPagedOrdersAsync(parameters, cancellationToken);
            var dtoList = pagedOrders.Items.Select(o =>
            {
                var dto = mapper.Map<OrderSummaryDto>(o);

                if (o.Status?.Name == "AwaitingPayment")
                {
                    dto.PaymentUrl = liqPayService.GeneratePaymentUrl(
                        o.Id,
                        o.TotalPrice,
                        $"Оплата замовлення #{o.Id}");
                }

                return dto;
            }).ToList();

            return new PagedList<OrderSummaryDto>(dtoList, pagedOrders.TotalCount, pagedOrders.CurrentPage, pagedOrders.PageSize);
        }

        public async Task<bool> HasUserOrderedBouquetAsync(Guid userId, Guid bouquetId)
        {
            var parameters = new OrderSpecificationParameters
            {
                UserId = userId,
                BouquetId = bouquetId
            };

            var orders = await unitOfWork.Orders.GetPagedOrdersAsync(parameters);
            var hasActiveOrders = orders.Items.Any(o => o.Status.Name == "Completed");

            return hasActiveOrders;
        }


        public async Task<OrderDetailDto> UpdateStatusAsync(Guid orderId, OrderUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var order = await unitOfWork.Orders.GetByIdWithIncludesAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Замовлення з ID {orderId} не знайдено");

            var status = await unitOfWork.OrderStatuses.GetByIdAsync(dto.StatusId);
            if (status == null)
                throw new NotFoundException($"Статус замовлення з ID {dto.StatusId} не знайдено");

            order.StatusId = dto.StatusId;
            unitOfWork.Orders.Update(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var dtoResult = mapper.Map<OrderDetailDto>(order);
            dtoResult.TotalPrice = order.Items.Sum(i => i.Price * i.Count);

            return dtoResult;
        }
    }
}