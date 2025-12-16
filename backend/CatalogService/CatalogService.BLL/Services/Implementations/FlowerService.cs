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
        private IUnitOfWork uow;
        private IMapper mapper;
        private IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService;

        public FlowerService(IUnitOfWork uow, IMapper mapper, IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.entityCacheInvalidationService = entityCacheInvalidationService;
        }

        public async Task<IEnumerable<FlowerDto>> GetAllAsync()
        {
            IEnumerable<Flower> flowers = await uow.Flowers.GetAllAsync();
            return mapper.Map<IEnumerable<FlowerDto>>(flowers);
        }

        public async Task<FlowerDto> GetByIdAsync(Guid id)
        {
            Flower? flower = await uow.Flowers.GetByIdAsync(id);
            if (flower == null)
                throw new NotFoundException($"Квітка з ID {id} не знайдена.");

            return mapper.Map<FlowerDto>(flower);
        }

        public async Task<FlowerDto> CreateAsync(FlowerCreateUpdateDto dto)
        {
            if (await uow.Flowers.ExistsWithNameAsync(dto.Name))
                throw new AlreadyExistsException($"Квітка '{dto.Name}' уже існує.");

            if (dto.Quantity < 0)
                throw new ArgumentException("Кількість повинна бути невід’ємною.");

            Flower flower = new Flower
            {
                Name = dto.Name,
                Color = dto.Color,
                Description = dto.Description,
                Quantity = dto.Quantity
            };

            await uow.Flowers.AddAsync(flower);
            await uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<FlowerDto>(flower);
        }

        public async Task<FlowerDto> UpdateAsync(Guid id, FlowerCreateUpdateDto dto)
        {
            Flower? flower = await uow.Flowers.GetByIdAsync(id);
            if (flower == null)
                throw new NotFoundException($"Квітка з ID {id} не знайдена.");

            if (await uow.Flowers.ExistsWithNameAsync(dto.Name, id))
                throw new AlreadyExistsException($"Квітка '{dto.Name}' уже існує.");

            if (dto.Quantity < 0)
                throw new ArgumentException("Кількість повинна бути невід’ємною.");

            flower.Name = dto.Name;
            flower.Color = dto.Color;
            flower.Description = dto.Description;
            flower.Quantity = dto.Quantity;

            uow.Flowers.Update(flower);
            await uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<FlowerDto>(flower);
        }

        public async Task DeleteAsync(Guid id)
        {
            Flower? flower = await uow.Flowers.GetByIdAsync(id);
            if (flower == null)
                throw new NotFoundException($"Квітка з ID {id} не знайдена.");

            uow.Flowers.Delete(flower);
            await uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();
        }
    }

}
