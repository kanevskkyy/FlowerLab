using AutoMapper;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Exceptions;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using shared.cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Implementations
{
    public class SizeService : ISizeService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService;

        public SizeService(IUnitOfWork uow, IMapper mapper, IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService)
        {
            _uow = uow;
            _mapper = mapper;
            this.entityCacheInvalidationService = entityCacheInvalidationService;
        }

        public async Task<IEnumerable<SizeDto>> GetAllAsync()
        {
            var sizes = await _uow.Sizes.GetAllAsync();
            return _mapper.Map<IEnumerable<SizeDto>>(sizes);
        }

        public async Task<SizeDto> GetByIdAsync(Guid id)
        {
            var size = await _uow.Sizes.GetByIdAsync(id);
            if (size == null) throw new NotFoundException($"Розмір з ID {id} не знайдений");
            return _mapper.Map<SizeDto>(size);
        }

        public async Task<SizeDto> CreateAsync(string name)
        {
            if (await _uow.Sizes.ExistsWithNameAsync(name))
                throw new AlreadyExistsException($"Розмір '{name}' уже існує.");

            var entity = new Size { Name = name };
            await _uow.Sizes.AddAsync(entity);
            await _uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();

            return _mapper.Map<SizeDto>(entity);
        }

        public async Task<SizeDto> UpdateAsync(Guid id, string name)
        {
            var size = await _uow.Sizes.GetByIdAsync(id);
            if (size == null) throw new NotFoundException($"Розмір з ID {id} не знайдений");

            if (await _uow.Sizes.ExistsWithNameAsync(name, id))
                throw new AlreadyExistsException($"Розмір '{name}' уже існує.");

            size.Name = name;
            _uow.Sizes.Update(size);
            await _uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();

            return _mapper.Map<SizeDto>(size);
        }

        public async Task DeleteAsync(Guid id)
        {
            var size = await _uow.Sizes.GetByIdAsync(id);
            if (size == null) throw new NotFoundException($"Розмір з ID {id} не знайдений");

            _uow.Sizes.Delete(size);
            await _uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();
        }
    }
}
