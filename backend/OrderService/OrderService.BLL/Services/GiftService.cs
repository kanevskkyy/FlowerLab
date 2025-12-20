using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OrderService.BLL.DTOs.GiftsDTOs;
using OrderService.BLL.Exceptions;
using OrderService.BLL.Helpers;
using OrderService.BLL.Services.Interfaces;
using OrderService.DAL.UOW;
using OrderService.Domain.Entities;

namespace OrderService.BLL.Services
{
    public class GiftService : IGiftService
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;
        private IImageService imageService;

        public GiftService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.imageService = imageService;
        }

        public async Task<IEnumerable<GiftReadDto>> GetAllAsync()
        {
            var gifts = await unitOfWork.Gifts.GetAllAsync();
            return mapper.Map<IEnumerable<GiftReadDto>>(gifts);
        }

        public async Task<GiftReadDto> GetByIdAsync(Guid id)
        {
            var gift = await unitOfWork.Gifts.GetByIdAsync(id);
            if (gift == null)
                throw new NotFoundException($"Gift with ID {id} was not found");

            return mapper.Map<GiftReadDto>(gift);
        }

        public async Task<GiftReadDto> CreateAsync(GiftCreateDto dto)
        {
            var isDuplicate = await unitOfWork.Gifts.IsNameDuplicatedAsync(dto.Name);
            if (isDuplicate)
                throw new AlreadyExistsException($"Gift '{dto.Name}' already exists");

            string imageUrl = await imageService.UploadAsync(dto.Image, "order-service/gifts");

            var entity = mapper.Map<Gift>(dto);
            entity.ImageUrl = imageUrl;
            entity.CreatedAt = DateTime.UtcNow.ToUniversalTime();

            await unitOfWork.Gifts.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<GiftReadDto>(entity);
        }

        public async Task<GiftReadDto> UpdateAsync(Guid id, GiftUpdateDto dto)
        {
            var gift = await unitOfWork.Gifts.GetByIdAsync(id);
            if (gift == null)
                throw new NotFoundException($"Gift with ID {id} was not found");

            var isDuplicate = await unitOfWork.Gifts.IsNameDuplicatedAsync(dto.Name, id);
            if (isDuplicate)
                throw new AlreadyExistsException($"Gift '{dto.Name}' already exists");

            if (dto.Image != null)
            {
                await imageService.DeleteImageAsync(gift.ImageUrl);
                string newImageUrl = await imageService.UploadAsync(dto.Image, "order-service/gifts");
                gift.ImageUrl = newImageUrl;
            }

            mapper.Map(dto, gift);
            unitOfWork.Gifts.Update(gift);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<GiftReadDto>(gift);
        }

        public async Task DeleteAsync(Guid id)
        {
            var gift = await unitOfWork.Gifts.GetByIdAsync(id);
            if (gift == null)
                throw new NotFoundException($"Gift with ID {id} was not found");

            await imageService.DeleteImageAsync(gift.ImageUrl);

            unitOfWork.Gifts.Delete(gift);
            await unitOfWork.SaveChangesAsync();
        }

    }
}
