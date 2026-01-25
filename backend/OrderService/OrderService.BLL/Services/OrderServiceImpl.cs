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
using shared.cache;
using shared.events.EmailEvents;
using shared.events.OrderEvents;
using shared.events.TelegramBotEvent;
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
        private readonly IEntityCacheInvalidationService<Gift> cacheInvalidationService;
        private readonly IEntityCacheInvalidationService<OrderStatus> orderStatusCacheInvalidator;


        public OrderServiceImpl(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CheckOrder.CheckOrderClient catalogClient,
            IPublishEndpoint publishEndpoint,
            ILiqPayService liqPayService,
            IEntityCacheInvalidationService<Gift> cacheInvalidationService,
            IEntityCacheInvalidationService<OrderStatus> orderStatusCacheInvalidator)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.catalogClient = catalogClient;
            this.publishEndpoint = publishEndpoint;
            this.liqPayService = liqPayService;
            this.cacheInvalidationService = cacheInvalidationService;
            this.orderStatusCacheInvalidator = orderStatusCacheInvalidator;
        }

        public async Task<string> GeneratePaymentUrlAsync(Guid orderId, Guid? guestToken = null, CancellationToken cancellationToken = default)
        {
            var order = await unitOfWork.Orders.GetByIdWithIncludesAsync(orderId, cancellationToken)
                ?? throw new NotFoundException($"Order {orderId} was not found");

            if (order.UserId == null && guestToken != order.GuestToken)
                throw new ValidationException("Invalid token for guest order.");

            if (order.Status.Name != "AwaitingPayment")
                throw new ValidationException("Order is already paid or not available for payment.");

            return liqPayService.GeneratePaymentUrl(
                order.Id,
                order.TotalPrice,
                $"Оплата за замовлення #{order.Id}");
        }


        public async Task<PagedList<OrderSummaryDto>> GetPagedOrdersAsync(OrderSpecificationParameters parameters, CancellationToken cancellationToken = default)
        {
            var pagedOrders = await unitOfWork.Orders.GetPagedOrdersAsync(parameters, cancellationToken);
            var dtoList = mapper.Map<List<OrderSummaryDto>>(pagedOrders.Items);

            return PagedList<OrderSummaryDto>.Create(dtoList, pagedOrders.TotalCount, pagedOrders.CurrentPage, pagedOrders.PageSize);
        }

        public async Task<OrderDetailDto> GetByIdAsync(Guid orderId, Guid? guestToken = null, CancellationToken cancellationToken = default)
        {
            var order = await unitOfWork.Orders.GetByIdWithIncludesAsync(orderId, cancellationToken)
                ?? throw new NotFoundException($"Order with ID {orderId} was not found");

            return mapper.Map<OrderDetailDto>(order);
        }

        public async Task<OrderDetailDto> CreateAsync(Guid? userId, string? userFirstName, string? userLastName, string? userPhoneNumber, string? userPhotoUrl, OrderCreateDto dto, decimal personalDiscount, CancellationToken cancellationToken = default)
        {
            string? finalFirstName = userFirstName ?? dto.FirstName;
            string? finalLastName = userLastName ?? dto.LastName;
            string? finalPhoneNumber = userPhoneNumber ?? dto.PhoneNumber;

            var now = DateTime.UtcNow;
            var activeReservations = await unitOfWork.OrderReservations.GetActiveAsync(now, cancellationToken);

            var grpcRequest = new OrderedBouquetsIdList();
            foreach (var item in dto.Items)
            {
                var reservedCount = activeReservations
                                    .Where(r => r.BouquetId == item.BouquetId && r.SizeId == item.SizeId)
                                    .Sum(r => r.Quantity);

                grpcRequest.OrderedBouquets.Add(new OrderedBouquetsId
                {
                    Id = item.BouquetId.ToString(),
                    SizeId = item.SizeId.ToString(),
                    Count = item.Count + reservedCount
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
                throw new ValidationException($"Error checking bouquets: {ex.Status.Detail}");
            }

            var invalidItems = catalogResponse.OrderedResponseList_
                .Where(r => !r.IsValid)
                .ToList();

            if (invalidItems.Any())
            {
                var errors = string.Join("; ", invalidItems.Select(i =>
                    $"Bouquet '{i.BouquetName}' (size {i.SizeName}): {i.ErrorMessage}"));
                throw new ValidationException($"Error checking bouquets: {errors}");
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
                await orderStatusCacheInvalidator.InvalidateAllAsync();
            }


            bool isFirstOrder = false;
            if (userId.HasValue)
            {
                isFirstOrder = await CheckDiscountEligibilityAsync(userId.Value, cancellationToken);
            }

            if (dto.Gifts != null && dto.Gifts.GroupBy(g => g.GiftId).Any(g => g.Count() > 1))
                throw new ValidationException("Duplicate gifts are not allowed in the order.");

            List<OrderGift> orderGifts = new();
            if (dto.Gifts != null)
            {
                foreach (var giftDto in dto.Gifts)
                {
                    var gift = await unitOfWork.Gifts.GetByIdAsync(giftDto.GiftId)
                        ?? throw new NotFoundException($"Gift with ID {giftDto.GiftId} was not found");

                    if (gift.AvailableCount < giftDto.Count)
                        throw new ValidationException($"Not enough gifts '{gift.Name}'. Requested {giftDto.Count}, available {gift.AvailableCount}.");

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
                    throw new ValidationException($"Invalid price for bouquet {catalogItem.BouquetName}");

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
                UserPhotoUrl = userPhotoUrl,
                PickupStoreAddress = dto.PickupStoreAddress,
                Notes = dto.Notes,
                PhoneNumber = finalPhoneNumber,
                ReceiverName = dto.ReceiverName,
                ReceiverPhone = dto.ReceiverPhone,
                GiftMessage = dto.GiftMessage,
                StatusId = awaitingPaymentStatus.Id,
                IsDelivery = dto.IsDelivery,
                Items = orderItems,
                DeliveryInformation = dto.DeliveryInformation != null
                    ? mapper.Map<DeliveryInformation>(dto.DeliveryInformation)
                    : null,
                OrderGifts = orderGifts
            };

            if (userId == null)
            {
                order.GuestToken = dto.GuestToken ?? Guid.NewGuid();
            }

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

            now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(10);

            foreach (var item in order.Items)
            {
                order.Reservations.Add(new OrderReservation
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    BouquetId = item.BouquetId,
                    BouquetName = item.BouquetName,
                    SizeId = item.SizeId,
                    SizeName = item.SizeName,
                    Quantity = item.Count,
                    ReservedAt = now,
                    ExpiresAt = expiresAt,
                    IsActive = true
                });
            }

            foreach (var orderGift in order.OrderGifts)
            {
                order.GiftReservations.Add(new GiftReservation
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    GiftId = orderGift.GiftId,
                    Quantity = orderGift.Count,
                    ReservedAt = now,
                    ExpiresAt = expiresAt,
                    IsActive = true
                });
            }

            await unitOfWork.Orders.AddAsync(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<OrderDetailDto>(order);
        }

        public async Task ProcessPaymentCallbackAsync(string data, string signature, CancellationToken cancellationToken = default)
        {
            if (!liqPayService.ValidateCallback(data, signature)) return;

            var response = liqPayService.ParseCallback(data);
            var orderId = Guid.Parse(response.OrderId);

            var order = await unitOfWork.Orders.GetByIdWithIncludesAsync(orderId, cancellationToken)
                ?? throw new NotFoundException($"Order {orderId} was not found");

            if (response.Status == LiqPayResponseStatus.Success || response.Status == LiqPayResponseStatus.Sandbox)
            {
                var paidStatus = await unitOfWork.OrderStatuses.GetByNameAsync("Pending")
                    ?? throw new NotFoundException("Status 'Pending' was not found");

                if (order.Status.Name != "Pending")
                {
                    order.StatusId = paidStatus.Id;
                    order.Status = paidStatus;
                    order.UpdatedAt = DateTime.UtcNow.ToUniversalTime();

                    await FinalizeOrderAsync(order, cancellationToken);

                    unitOfWork.Orders.Update(order);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
        }

        private async Task FinalizeOrderAsync(Order order, CancellationToken cancellationToken)
        {
            // 1. Deduct Gift Stock
            foreach (var orderGift in order.OrderGifts)
            {
                var gift = await unitOfWork.Gifts.GetByIdAsync(orderGift.GiftId);
                if (gift != null)
                {
                    if (gift.AvailableCount < orderGift.Count)
                        throw new ValidationException($"Not enough gifts '{gift.Name}' to complete the order.");

                    gift.AvailableCount -= orderGift.Count;
                    gift.UpdatedAt = DateTime.UtcNow.ToUniversalTime();
                    unitOfWork.Gifts.Update(gift);
                    await cacheInvalidationService.InvalidateByIdAsync(gift.Id);
                }
            }

            // 2. Deactivate Reservations
            foreach (var reservation in order.Reservations)
            {
                reservation.IsActive = false;
                unitOfWork.OrderReservations.Update(reservation);
            }

            foreach (var giftReservation in order.GiftReservations)
            {
                giftReservation.IsActive = false;
                unitOfWork.GiftReservations.Update(giftReservation);
            }

            // 3. Publish Events (Flower deduction happens in CatalogService consumer)
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                Bouquets = order.Items.Select(i => new OrderBouquetItem
                {
                    BouquetId = i.BouquetId,
                    SizeId = i.SizeId,
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

            if (order.IsDelivery && order.DeliveryInformation != null)
            {
                OrderAddressEvent orderAddressEvent = new OrderAddressEvent()
                {
                    Address = order.DeliveryInformation.Address,
                    UserId = order.UserId.ToString()
                };
                await publishEndpoint.Publish(orderAddressEvent, cancellationToken);
            }

            await cacheInvalidationService.InvalidateAllAsync();
        }

        public async Task<PagedList<OrderSummaryDto>> GetMyOrdersAsync(Guid? userId, Guid? guestToken, OrderSpecificationParameters parameters, CancellationToken cancellationToken = default)
        {
            parameters.UserId = userId;
            parameters.GuestToken = guestToken;

            var pagedOrders = await unitOfWork.Orders.GetPagedOrdersAsync(parameters, cancellationToken);

            var dtoList = mapper.Map<List<OrderSummaryDto>>(pagedOrders.Items);
            return PagedList<OrderSummaryDto>.Create(dtoList, pagedOrders.TotalCount, pagedOrders.CurrentPage, pagedOrders.PageSize);
        }

        public async Task<bool> HasUserOrderedBouquetAsync(Guid userId, Guid bouquetId)
        {
            return await unitOfWork.Orders.HasUserOrderedBouquetAsync(userId, bouquetId);
        }


        public async Task<OrderDetailDto> UpdateStatusAsync(Guid orderId, OrderUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var order = await unitOfWork.Orders.GetByIdWithIncludesAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Order with ID {orderId} was not found");

            var status = await unitOfWork.OrderStatuses.GetByIdAsync(dto.StatusId);
            if (status == null)
                throw new NotFoundException($"Order status with ID {dto.StatusId} was not found");

            var oldStatusName = order.Status.Name;
            order.StatusId = dto.StatusId;
            order.Status = status;

            // Trigger stock deduction and events if status changes to Pending (manual payment confirmation)
            if (oldStatusName != "Pending" && status.Name == "Pending")
            {
                await FinalizeOrderAsync(order, cancellationToken);
            }

            unitOfWork.Orders.Update(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var dtoResult = mapper.Map<OrderDetailDto>(order);
            dtoResult.TotalPrice = order.Items.Sum(i => i.Price * i.Count);

            return dtoResult;
        }

        public async Task<bool> CheckDiscountEligibilityAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var pagedOrders = await unitOfWork.Orders.GetPagedOrdersAsync(
                new OrderSpecificationParameters { UserId = userId, PageSize = 100 },
                cancellationToken);

            Console.WriteLine($"[CheckDiscount] User {userId} has {pagedOrders.TotalCount} total orders.");

            var consumedStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Completed" };

            bool hasSuccessfulOrders = pagedOrders.Items.Any(o => 
            {
                Console.WriteLine($"[CheckDiscount] Order {o.Id} Status: {o.Status?.Name}");
                return o.Status != null && consumedStatuses.Contains(o.Status.Name);
            });

            Console.WriteLine($"[CheckDiscount] Has Successful Orders: {hasSuccessfulOrders}. Eligible: {!hasSuccessfulOrders}");

            return !hasSuccessfulOrders;
        }

        public async Task DeleteAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await unitOfWork.Orders.GetByIdWithIncludesAsync(orderId, cancellationToken)
                ?? throw new NotFoundException($"Order {orderId} was not found");

            // Option 1: Only allow deleting AwaitingPayment orders
            // if (order.Status.Name != "AwaitingPayment")
            //     throw new ValidationException("Only unpaid orders can be deleted.");

            foreach (var reservation in order.Reservations)
            {
                unitOfWork.OrderReservations.Delete(reservation);
            }

            foreach (var giftReservation in order.GiftReservations)
            {
                unitOfWork.GiftReservations.Delete(giftReservation);
                await cacheInvalidationService.InvalidateByIdAsync(giftReservation.GiftId);
            }

            unitOfWork.Orders.Delete(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

    }
}