using AutoMapper;
using OrderService.BLL.DTOs.OrderStatusDTOs;
using OrderService.BLL.Exceptions;
using OrderService.BLL.Services.Interfaces;
using OrderService.DAL.UOW;
using OrderService.Domain.Entities;
using shared.cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IEntityCacheService cacheService;
        private readonly IEntityCacheInvalidationService<OrderStatus> cacheInvalidationService;

        private const string ALL_STATUSES_KEY = "order-status:all:v3";
        private static readonly TimeSpan MEMORY_TTL = TimeSpan.FromHours(1);
        private static readonly TimeSpan REDIS_TTL = TimeSpan.FromDays(7);

        public OrderStatusService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEntityCacheService cacheService,
            IEntityCacheInvalidationService<OrderStatus> cacheInvalidationService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.cacheService = cacheService;
            this.cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<IEnumerable<OrderStatusReadDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var cached = await cacheService.GetOrSetAsync(
                ALL_STATUSES_KEY,
                async () =>
                {
                    var statuses = await unitOfWork.OrderStatuses.GetAllAsync(cancellationToken);
                    return mapper.Map<List<OrderStatusReadDto>>(statuses.ToList());
                },
                MEMORY_TTL,
                REDIS_TTL
            );

            return cached!;
        }

        public async Task<OrderStatusReadDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"order-status:{id}";

            var cached = await cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var status = await unitOfWork.OrderStatuses.GetByIdAsync(id, cancellationToken);
                    if (status == null)
                        throw new NotFoundException($"Order status with ID {id} was not found");

                    return mapper.Map<OrderStatusReadDto>(status);
                },
                MEMORY_TTL,
                REDIS_TTL
            );

            return cached!;
        }

        public async Task<OrderStatusReadDto> CreateAsync(OrderStatusCreateDto dto, CancellationToken cancellationToken = default)
        {
            bool isDuplicate = await unitOfWork.OrderStatuses.IsNameDuplicatedAsync(dto.Name);
            if (isDuplicate)
                throw new AlreadyExistsException($"Order status '{dto.Name}' already exists");

            var entity = mapper.Map<OrderStatus>(dto);

            await unitOfWork.OrderStatuses.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();

            await cacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<OrderStatusReadDto>(entity);
        }

        public async Task<OrderStatusReadDto> UpdateAsync(Guid id, OrderStatusUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var status = await unitOfWork.OrderStatuses.GetByIdAsync(id, cancellationToken);
            if (status == null)
                throw new NotFoundException($"Order status with ID {id} was not found");

            bool isDuplicate = await unitOfWork.OrderStatuses.IsNameDuplicatedAsync(dto.Name, id);
            if (isDuplicate)
                throw new AlreadyExistsException($"Order status '{dto.Name}' already exists");

            mapper.Map(dto, status);
            unitOfWork.OrderStatuses.Update(status);
            await unitOfWork.SaveChangesAsync();

            await cacheInvalidationService.InvalidateByIdAsync(id);
            await cacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<OrderStatusReadDto>(status);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var status = await unitOfWork.OrderStatuses.GetByIdAsync(id, cancellationToken);
            if (status == null)
                throw new NotFoundException($"Order status with ID {id} was not found");

            unitOfWork.OrderStatuses.Delete(status);
            await unitOfWork.SaveChangesAsync();

            await cacheInvalidationService.InvalidateByIdAsync(id);
            await cacheInvalidationService.InvalidateAllAsync();
        }
    }
}
