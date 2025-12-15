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
    public class FlowerService : IFlowerService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService;

        public FlowerService(IUnitOfWork uow, IMapper mapper, IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService)
        {
            _uow = uow;
            _mapper = mapper;
            this.entityCacheInvalidationService = entityCacheInvalidationService;
        }

        public async Task<IEnumerable<FlowerDto>> GetAllAsync()
        {
            var flowers = await _uow.Flowers.GetAllAsync();
            return _mapper.Map<IEnumerable<FlowerDto>>(flowers);
        }

        public async Task<FlowerDto> GetByIdAsync(Guid id)
        {
            var flower = await _uow.Flowers.GetByIdAsync(id);
            if (flower == null)
                throw new NotFoundException($"Квітка з ID {id} не знайдена.");

            return _mapper.Map<FlowerDto>(flower);
        }

        public async Task<FlowerDto> CreateAsync(FlowerCreateUpdateDto dto)
        {
            if (await _uow.Flowers.ExistsWithNameAsync(dto.Name))
                throw new AlreadyExistsException($"Квітка '{dto.Name}' уже існує.");

            if (dto.Quantity < 0)
                throw new ArgumentException("Кількість повинна бути невід’ємною.");

            var flower = new Flower
            {
                Name = dto.Name,
                Color = dto.Color,
                Description = dto.Description,
                Quantity = dto.Quantity
            };

            await _uow.Flowers.AddAsync(flower);
            await _uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();

            return _mapper.Map<FlowerDto>(flower);
        }

        public async Task<FlowerDto> UpdateAsync(Guid id, FlowerCreateUpdateDto dto)
        {
            var flower = await _uow.Flowers.GetByIdAsync(id);
            if (flower == null)
                throw new NotFoundException($"Квітка з ID {id} не знайдена.");

            if (await _uow.Flowers.ExistsWithNameAsync(dto.Name, id))
                throw new AlreadyExistsException($"Квітка '{dto.Name}' уже існує.");

            if (dto.Quantity < 0)
                throw new ArgumentException("Кількість повинна бути невід’ємною.");

            flower.Name = dto.Name;
            flower.Color = dto.Color;
            flower.Description = dto.Description;
            flower.Quantity = dto.Quantity;

            _uow.Flowers.Update(flower);
            await _uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();

            return _mapper.Map<FlowerDto>(flower);
        }

        public async Task DeleteAsync(Guid id)
        {
            var flower = await _uow.Flowers.GetByIdAsync(id);
            if (flower == null)
                throw new NotFoundException($"Квітка з ID {id} не знайдена.");

            _uow.Flowers.Delete(flower);
            await _uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();
        }

        public async Task<FlowerDto> UpdateStockAsync(Guid id, int quantity)
        {
            var flower = await _uow.Flowers.GetByIdAsync(id);
            if (flower == null)
                throw new NotFoundException($"Квітка з ID {id} не знайдена.");

            if (quantity < 0)
                throw new ArgumentException("Кількість повинна бути невід’ємною.");

            flower.Quantity = quantity;
            _uow.Flowers.Update(flower);
            await _uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();

            return _mapper.Map<FlowerDto>(flower);
        }
    }

}
