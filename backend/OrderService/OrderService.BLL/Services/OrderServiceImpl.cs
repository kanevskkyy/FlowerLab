using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OrderService.BLL.DTOs.OrderDTOs;
using OrderService.BLL.Exceptions;
using OrderService.BLL.Services.Interfaces;
using OrderService.DAL.Helpers;
using OrderService.DAL.Specification;
using OrderService.DAL.UOW;
using OrderService.Domain.Entities;
using OrderService.Domain.QueryParams;
using Grpc.Core;
using shared.events;
using MassTransit;

namespace OrderService.BLL.Services
{
    public class OrderServiceImpl : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CheckOrder.CheckOrderClient _catalogClient;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderServiceImpl(IUnitOfWork unitOfWork, IMapper mapper, CheckOrder.CheckOrderClient catalogClient, IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _catalogClient = catalogClient;
        }

        public async Task<PagedList<OrderSummaryDto>> GetPagedOrdersAsync(OrderSpecificationParameters parameters, CancellationToken cancellationToken = default)
        {
            var pagedOrders = await _unitOfWork.Orders.GetPagedOrdersAsync(parameters, cancellationToken);
            var dtoList = pagedOrders.Items.Select(o => _mapper.Map<OrderSummaryDto>(o)).ToList();
            return new PagedList<OrderSummaryDto>(dtoList, pagedOrders.TotalCount, pagedOrders.CurrentPage, pagedOrders.PageSize);
        }

        public async Task<OrderDetailDto> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.Orders.GetByIdWithIncludesAsync(orderId, cancellationToken);
            if (order == null)
                throw new NotFoundException($"Замовлення з ID {orderId} не знайдено");

            return _mapper.Map<OrderDetailDto>(order);
        }

        public async Task<OrderDetailDto> CreateAsync(Guid? userId, string? userFirstName, string? userLastName, OrderCreateDto dto, decimal personalDiscount, CancellationToken cancellationToken = default)
        {
            var finalFirstName = userFirstName ?? dto.FirstName;
            var finalLastName = userLastName ?? dto.LastName;

            var grpcRequest = new OrderedBouquetsIdList();
            foreach (var item in dto.Items)
            {
                grpcRequest.OrderedBouquets.Add(new OrderedBouquetsId
                {
                    Id = item.BouquetId.ToString(),
                    Count = item.Count
                });
            }

            OrderedResponseList catalogResponse;
            try
            {
                catalogResponse = await _catalogClient.CheckOrderItemsAsync(
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
                    $"Букет '{i.BouquetName}': {i.ErrorMessage}"));
                throw new ValidationException($"Помилка перевірки букетів: {errors}");
            }

            var pendingStatus = await _unitOfWork.OrderStatuses.GetByNameAsync("Pending");
            if (pendingStatus == null)
            {
                pendingStatus = new OrderStatus
                {
                    Name = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.OrderStatuses.AddAsync(pendingStatus);
            }

            var existingOrders = await _unitOfWork.Orders.GetPagedOrdersAsync(
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
                    var gift = await _unitOfWork.Gifts.GetByIdAsync(giftDto.GiftId)
                               ?? throw new NotFoundException($"Подарунок з ID {giftDto.GiftId} не знайдено");

                    if (gift.AvailableCount < giftDto.Count)
                        throw new ValidationException(
                            $"Недостатньо подарунків '{gift.Name}'. Запитано {giftDto.Count}, " +
                            $"доступно {gift.AvailableCount}.");

                    gift.AvailableCount -= giftDto.Count;
                    gift.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Gifts.Update(gift);

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
                    throw new ValidationException(
                        $"Некоректна ціна для букета {catalogItem.BouquetName}");

                orderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    BouquetId = itemDto.BouquetId,
                    BouquetName = catalogItem.BouquetName,
                    BouquetImage = catalogItem.BouquetImage,
                    Price = price,
                    Count = itemDto.Count
                });
            }

            var order = new Order
            {
                UserId = userId,                  
                UserFirstName = finalFirstName,    
                UserLastName = finalLastName,      
                Notes = dto.Notes,
                GiftMessage = dto.GiftMessage,
                StatusId = pendingStatus.Id,
                IsDelivery = dto.IsDelivery,
                Items = orderItems,
                DeliveryInformation = dto.DeliveryInformation != null
                    ? _mapper.Map<DeliveryInformation>(dto.DeliveryInformation)
                    : null,
                OrderGifts = orderGifts
            };

            decimal itemsTotal = order.Items.Sum(i => i.Price * i.Count);

            if (!string.IsNullOrWhiteSpace(order.GiftMessage)) itemsTotal += 50m;
            if (isFirstOrder) itemsTotal *= 0.9m;

            if (personalDiscount > 0) itemsTotal *= 1 - personalDiscount;

            order.TotalPrice = itemsTotal;

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                Bouquets = order.Items.Select(i => new OrderBouquetItem
                {
                    BouquetId = i.BouquetId,
                    Count = i.Count
                }).ToList()
            };
            await _publishEndpoint.Publish(orderCreatedEvent, cancellationToken);

            var resultDto = _mapper.Map<OrderDetailDto>(order);
            resultDto.TotalPrice = order.TotalPrice;

            return resultDto;
        }


        public async Task<OrderDetailDto> UpdateStatusAsync(Guid orderId, OrderUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Замовлення з ID {orderId} не знайдено");

            var status = await _unitOfWork.OrderStatuses.GetByIdAsync(dto.StatusId);
            if (status == null)
                throw new NotFoundException($"Статус замовлення з ID {dto.StatusId} не знайдено");

            order.StatusId = dto.StatusId;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dtoResult = _mapper.Map<OrderDetailDto>(order);
            dtoResult.TotalPrice = order.Items.Sum(i => i.Price * i.Count);

            return dtoResult;
        }
    }
}