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
    public class GiftService : IGiftService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public GiftService(IUnitOfWork uow, IMapper mapper, IImageService imageService)
        {
            _uow = uow;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<IEnumerable<GiftDto>> GetAllAsync()
        {
            var gifts = await _uow.Gifts.GetAllAsync();
            return _mapper.Map<IEnumerable<GiftDto>>(gifts);
        }

        public async Task<GiftDto> GetByIdAsync(Guid id)
        {
            var gift = await _uow.Gifts.GetByIdAsync(id);
            if (gift == null) throw new NotFoundException($"Gift {id} not found");
            return _mapper.Map<GiftDto>(gift);
        }

        public async Task<GiftDto> CreateAsync(string name, string giftType, FileContentDto? image = null)
        {
            if (await _uow.Gifts.ExistsWithNameAsync(name))
                throw new AlreadyExistsException($"Gift '{name}' already exists.");

            string? imageUrl = null;
            if (image != null)
                imageUrl = await _imageService.UploadAsync(image.Content, image.FileName, "gifts");

            var gift = new Gift
            {
                Name = name,
                GiftType = giftType,
                ImageUrl = imageUrl
            };

            await _uow.Gifts.AddAsync(gift);
            await _uow.SaveChangesAsync();

            return _mapper.Map<GiftDto>(gift);
        }

        public async Task<GiftDto> UpdateAsync(Guid id, string name, string giftType, FileContentDto? image = null)
        {
            var gift = await _uow.Gifts.GetByIdAsync(id);
            if (gift == null) throw new NotFoundException($"Gift {id} not found");

            if (await _uow.Gifts.ExistsWithNameAsync(name, id))
                throw new AlreadyExistsException($"Gift '{name}' already exists.");

            gift.Name = name;
            gift.GiftType = giftType;

            if (image != null)
                gift.ImageUrl = await _imageService.UploadAsync(image.Content, image.FileName, "gifts");

            _uow.Gifts.Update(gift);
            await _uow.SaveChangesAsync();

            return _mapper.Map<GiftDto>(gift);
        }

        public async Task DeleteAsync(Guid id)
        {
            var gift = await _uow.Gifts.GetByIdAsync(id);
            if (gift == null) throw new NotFoundException($"Gift {id} not found");

            _uow.Gifts.Delete(gift);
            await _uow.SaveChangesAsync();
        }
    }
}
