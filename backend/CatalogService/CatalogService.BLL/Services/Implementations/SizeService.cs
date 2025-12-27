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
    public class SizeService : ISizeService
    {
        private IUnitOfWork uow;
        private IMapper mapper;
        private IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService;

        public SizeService(IUnitOfWork uow, IMapper mapper, IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.entityCacheInvalidationService = entityCacheInvalidationService;
        }

        public async Task<IEnumerable<SizeDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Size> sizes = await uow.Sizes.GetAllAsync(cancellationToken);
            return mapper.Map<IEnumerable<SizeDto>>(sizes);
        }

        public async Task<SizeDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Size? size = await uow.Sizes.GetByIdAsync(id, cancellationToken);
            if (size == null)
                throw new NotFoundException($"Size with ID {id} not found.");

            return mapper.Map<SizeDto>(size);
        }

        public async Task<SizeDto> CreateAsync(string name, CancellationToken cancellationToken = default)
        {
            if (await uow.Sizes.ExistsWithNameAsync(name, cancellationToken: cancellationToken))
                throw new AlreadyExistsException($"Size '{name}' already exists.");

            Size entity = new Size { Name = name };
            await uow.Sizes.AddAsync(entity, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<SizeDto>(entity);
        }

        public async Task<SizeDto> UpdateAsync(Guid id, string name, CancellationToken cancellationToken = default)
        {
            Size? size = await uow.Sizes.GetByIdAsync(id, cancellationToken);
            if (size == null)
                throw new NotFoundException($"Size with ID {id} not found.");

            if (await uow.Sizes.ExistsWithNameAsync(name, id, cancellationToken))
                throw new AlreadyExistsException($"Size '{name}' already exists.");

            size.Name = name;
            uow.Sizes.Update(size);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<SizeDto>(size);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Size? size = await uow.Sizes.GetByIdAsync(id, cancellationToken);
            if (size == null)
                throw new NotFoundException($"Size with ID {id} not found.");

            uow.Sizes.Delete(size);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();
        }
    }
}
