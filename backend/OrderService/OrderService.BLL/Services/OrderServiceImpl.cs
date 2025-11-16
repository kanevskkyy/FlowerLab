using System;
using System.Collections.Generic;
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

namespace OrderService.BLL.Services
{
    public class OrderServiceImpl : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderServiceImpl(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
                throw new NotFoundException($"Order with ID {orderId} not found");

            var dto = _mapper.Map<OrderDetailDto>(order);
            dto.TotalPrice = order.Items.Sum(i => i.Price * i.Count);
            return dto;
        }

        public async Task<OrderDetailDto> CreateAsync(OrderCreateDto dto, CancellationToken cancellationToken = default)
        {
            var pendingStatus = await _unitOfWork.OrderStatuses.GetByNameAsync("Pending");
            if (pendingStatus == null)
            {
                var orderStatus = new OrderStatus { Name = "Pending", CreatedAt = DateTime.UtcNow.ToUniversalTime(), UpdatedAt = DateTime.UtcNow.ToUniversalTime() };
                await _unitOfWork.OrderStatuses.AddAsync(orderStatus);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                pendingStatus = orderStatus;
            }

            var existingOrders = await _unitOfWork.Orders.GetPagedOrdersAsync(
                new OrderSpecificationParameters { 
                    UserId = dto.UserId 
                }, cancellationToken);

            bool isFirstOrder = !existingOrders.Items.Any(o => o.Items.Any());

            var validGifts = new List<Gift>();
            if (dto.GiftIds != null && dto.GiftIds.Any())
            {
                foreach (var giftId in dto.GiftIds)
                {
                    var gift = await _unitOfWork.Gifts.GetByIdAsync(giftId);
                    if (gift == null)
                        throw new NotFoundException($"Gift with ID {giftId} not found");
                    validGifts.Add(gift);
                }
            }

            var order = new Order
            {
                UserId = dto.UserId,        
                UserFirstName = dto.UserFirstName,
                UserLastName = dto.UserLastName,
                Notes = dto.Notes,
                StatusId = pendingStatus.Id,
                IsDelivery = dto.IsDelivery,
                Items = dto.Items.Select(i => _mapper.Map<OrderItem>(i)).ToList(),
                DeliveryInformation = dto.DeliveryInformation != null ? _mapper.Map<DeliveryInformation>(dto.DeliveryInformation) : null
            };

            foreach (var gift in validGifts)
                order.OrderGifts.Add(new OrderGift { GiftId = gift.Id, Gift = gift });

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var resultDto = _mapper.Map<OrderDetailDto>(order);

            decimal itemsTotal = order.Items.Sum(i => i.Price * i.Count);
            if (!string.IsNullOrWhiteSpace(order.Notes))
            {
                itemsTotal += 50m;
            }
            if (isFirstOrder)
            {
                itemsTotal *= 0.9m;
            }

            resultDto.TotalPrice = itemsTotal;

            return resultDto;
        }

        public async Task<OrderDetailDto> UpdateStatusAsync(Guid orderId, OrderUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Order with ID {orderId} not found");

            var status = await _unitOfWork.OrderStatuses.GetByIdAsync(dto.StatusId);
            if (status == null)
                throw new NotFoundException($"OrderStatus with ID {dto.StatusId} not found");

            order.StatusId = dto.StatusId;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dtoResult = _mapper.Map<OrderDetailDto>(order);
            dtoResult.TotalPrice = order.Items.Sum(i => i.Price * i.Count);

            return dtoResult;
        }
    }
}
