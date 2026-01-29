using AutoMapper;
using OrderService.BLL.DTOs.GiftsDTOs;
using OrderService.BLL.Exceptions;
using OrderService.BLL.Helpers;
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
    public class GiftService : IGiftService
    {
        private readonly IUnitOfWork unitOfWork;
        private IMapper mapper;
        private readonly IImageService imageService;
        private readonly IEntityCacheService cacheService;
        private readonly IEntityCacheInvalidationService<Gift> cacheInvalidationService;

        private const string ALL_GIFTS_KEY = "gifts:all:v3";
        private static readonly TimeSpan MEMORY_TTL_GIFT = TimeSpan.FromHours(1);
        private static readonly TimeSpan REDIS_TTL_GIFT = TimeSpan.FromDays(7);

        public GiftService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IImageService imageService,
            IEntityCacheService cacheService,
            IEntityCacheInvalidationService<Gift> cacheInvalidationService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.imageService = imageService;
            this.cacheService = cacheService;
            this.cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<IEnumerable<GiftReadDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var cached = await cacheService.GetOrSetAsync(
                ALL_GIFTS_KEY,
                async () =>
                {
                    var gifts = await unitOfWork.Gifts.GetAllAsync(cancellationToken);
                    return mapper.Map<List<GiftReadDto>>(gifts.ToList());
                },
                MEMORY_TTL_GIFT,
                REDIS_TTL_GIFT
            );

            return cached!;
        }

        public async Task<GiftReadDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"gifts:{id}";

            var cached = await cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var gift = await unitOfWork.Gifts.GetByIdAsync(id, cancellationToken);
                    if (gift == null)
                        throw new NotFoundException($"Gift with ID {id} was not found");

                    return mapper.Map<GiftReadDto>(gift);
                },
                MEMORY_TTL_GIFT,
                REDIS_TTL_GIFT
            );

            return cached!;
        }

        public async Task<GiftReadDto> CreateAsync(GiftCreateDto dto, CancellationToken cancellationToken = default)
        {
            bool isDuplicate = await unitOfWork.Gifts.IsNameDuplicatedAsync(dto.Name.GetValueOrDefault("ua", ""));
            if (isDuplicate)
                throw new AlreadyExistsException($"Gift '{dto.Name.GetValueOrDefault("ua", "")}' already exists");

            string imageUrl = await imageService.UploadAsync(dto.Image, "order-service/gifts");

            var entity = mapper.Map<Gift>(dto);
            entity.ImageUrl = imageUrl;
            entity.CreatedAt = DateTime.UtcNow;

            await unitOfWork.Gifts.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();

            await cacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<GiftReadDto>(entity);
        }

        public async Task<GiftReadDto> UpdateAsync(Guid id, GiftUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var gift = await unitOfWork.Gifts.GetByIdAsync(id, cancellationToken);
            if (gift == null)
                throw new NotFoundException($"Gift with ID {id} was not found");

            bool isDuplicate = await unitOfWork.Gifts.IsNameDuplicatedAsync(dto.Name.GetValueOrDefault("ua", ""), id);
            if (isDuplicate)
                throw new AlreadyExistsException($"Gift '{dto.Name.GetValueOrDefault("ua", "")}' already exists");

            if (dto.Image != null)
            {
                await imageService.DeleteImageAsync(gift.ImageUrl);
                gift.ImageUrl = await imageService.UploadAsync(dto.Image, "order-service/gifts");
            }

            mapper.Map(dto, gift);
            gift.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Gifts.Update(gift);
            await unitOfWork.SaveChangesAsync();

            await cacheInvalidationService.InvalidateByIdAsync(id);
            await cacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<GiftReadDto>(gift);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gift = await unitOfWork.Gifts.GetByIdAsync(id, cancellationToken);
            if (gift == null)
                throw new NotFoundException($"Gift with ID {id} was not found");

            await imageService.DeleteImageAsync(gift.ImageUrl);

            unitOfWork.Gifts.Delete(gift);
            await unitOfWork.SaveChangesAsync();

            await cacheInvalidationService.InvalidateByIdAsync(id);
            await cacheInvalidationService.InvalidateAllAsync();
        }
    }

}
