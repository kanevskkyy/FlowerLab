using AutoMapper;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Exceptions;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
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

        public FlowerService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
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
                throw new NotFoundException($"Flower with id {id} not found.");

            return _mapper.Map<FlowerDto>(flower);
        }

        public async Task<FlowerDto> CreateAsync(FlowerCreateUpdateDto dto)
        {
            if (await _uow.Flowers.ExistsWithNameAsync(dto.Name))
                throw new AlreadyExistsException($"Flower '{dto.Name}' already exists.");

            if (dto.Quantity < 0)
                throw new ArgumentException("Quantity must be non-negative.");

            var flower = new Flower
            {
                Name = dto.Name,
                Color = dto.Color,
                Description = dto.Description,
                Quantity = dto.Quantity
            };

            await _uow.Flowers.AddAsync(flower);
            await _uow.SaveChangesAsync();

            return _mapper.Map<FlowerDto>(flower);
        }

        public async Task<FlowerDto> UpdateAsync(Guid id, FlowerCreateUpdateDto dto)
        {
            var flower = await _uow.Flowers.GetByIdAsync(id);
            if (flower == null)
                throw new NotFoundException($"Flower with id {id} not found.");

            if (await _uow.Flowers.ExistsWithNameAsync(dto.Name, id))
                throw new AlreadyExistsException($"Flower '{dto.Name}' already exists.");

            if (dto.Quantity < 0)
                throw new ArgumentException("Quantity must be non-negative.");

            flower.Name = dto.Name;
            flower.Color = dto.Color;
            flower.Description = dto.Description;
            flower.Quantity = dto.Quantity;

            _uow.Flowers.Update(flower);
            await _uow.SaveChangesAsync();

            return _mapper.Map<FlowerDto>(flower);
        }

        public async Task DeleteAsync(Guid id)
        {
            var flower = await _uow.Flowers.GetByIdAsync(id);
            if (flower == null)
                throw new NotFoundException($"Flower with id {id} not found.");

            _uow.Flowers.Delete(flower);
            await _uow.SaveChangesAsync();
        }

        public async Task<FlowerDto> UpdateStockAsync(Guid id, int quantity)
        {
            var flower = await _uow.Flowers.GetByIdAsync(id);
            if (flower == null)
                throw new NotFoundException($"Flower with id {id} not found.");

            if (quantity < 0)
                throw new ArgumentException("Quantity must be non-negative.");

            flower.Quantity = quantity;
            _uow.Flowers.Update(flower);
            await _uow.SaveChangesAsync();

            return _mapper.Map<FlowerDto>(flower);
        }
    }
}
