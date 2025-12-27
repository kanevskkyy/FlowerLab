using AutoMapper;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Exceptions;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using shared.cache;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Implementations
{
    public class FlowerService : IFlowerService
    {
        private IUnitOfWork uow;
        private IMapper mapper;
        private IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService;

        public FlowerService(IUnitOfWork uow, IMapper mapper, IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.entityCacheInvalidationService = entityCacheInvalidationService;
        }

        public async Task<IEnumerable<FlowerDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Flower> flowers = await uow.Flowers.GetAllAsync(cancellationToken);
            return mapper.Map<IEnumerable<FlowerDto>>(flowers);
        }

        public async Task<FlowerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Flower? flower = await uow.Flowers.GetByIdAsync(id, cancellationToken);
            if (flower == null)
                throw new NotFoundException($"Flower with ID {id} not found.");

            return mapper.Map<FlowerDto>(flower);
        }

        public async Task<FlowerDto> CreateAsync(FlowerCreateUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (await uow.Flowers.ExistsWithNameAsync(dto.Name, cancellationToken: cancellationToken))
                throw new AlreadyExistsException($"Flower '{dto.Name}' already exists.");

            if (dto.Quantity < 0)
                throw new ArgumentException("Quantity must be non-negative.");

            Flower flower = new Flower
            {
                Name = dto.Name,
                Color = dto.Color,
                Description = dto.Description,
                Quantity = dto.Quantity
            };

            await uow.Flowers.AddAsync(flower, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<FlowerDto>(flower);
        }

        public async Task<FlowerDto> UpdateAsync(Guid id, FlowerCreateUpdateDto dto, CancellationToken cancellationToken = default)
        {
            Flower? flower = await uow.Flowers.GetByIdAsync(id, cancellationToken);
            if (flower == null)
                throw new NotFoundException($"Flower with ID {id} not found.");

            if (await uow.Flowers.ExistsWithNameAsync(dto.Name, id, cancellationToken))
                throw new AlreadyExistsException($"Flower '{dto.Name}' already exists.");

            if (dto.Quantity < 0)
                throw new ArgumentException("Quantity must be non-negative.");

            flower.Name = dto.Name;
            flower.Color = dto.Color;
            flower.Description = dto.Description;
            flower.Quantity = dto.Quantity;

            uow.Flowers.Update(flower);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<FlowerDto>(flower);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Flower? flower = await uow.Flowers.GetByIdAsync(id, cancellationToken);
            if (flower == null)
                throw new NotFoundException($"Flower with ID {id} not found.");

            uow.Flowers.Delete(flower);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();
        }
    }
}
