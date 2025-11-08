using AutoMapper;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Exceptions;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using CatalogService.Domain.QueryParametrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Implementations
{
    public class BouquetService : IBouquetService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public BouquetService(IUnitOfWork uow, IMapper mapper, IImageService imageService)
        {
            _uow = uow;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<IEnumerable<BouquetDto>> GetAllAsync(BouquetQueryParameters parameters)
        {
            var bouquets = await _uow.Bouquets.GetBySpecificationAsync(parameters);
            return _mapper.Map<IEnumerable<BouquetDto>>(bouquets);
        }

        public async Task<BouquetDto> GetByIdAsync(Guid id)
        {
            var bouquet = await _uow.Bouquets.GetByIdAsync(id);
            if (bouquet == null) throw new NotFoundException($"Bouquet with id {id} not found.");
            return _mapper.Map<BouquetDto>(bouquet);
        }

        public async Task<BouquetDto> CreateAsync(BouquetCreateDto dto)
        {
            // перевірка унікальності
            if (await _uow.Bouquets.ExistsAsync(b => b.Name == dto.Name))
                throw new AlreadyExistsException($"Bouquet with name '{dto.Name}' already exists.");

            // Перевірка існування size
            var size = await _uow.Sizes.GetByIdAsync(dto.SizeId);
            if (size == null) throw new NotFoundException($"Size {dto.SizeId} not found.");

            // перевірити events
            foreach (var evId in dto.EventIds)
            {
                if (!await _uow.Events.ExistsAsync(e => e.Id == evId))
                    throw new NotFoundException($"Event {evId} not found.");
            }

            // recipients
            foreach (var rId in dto.RecipientIds)
            {
                if (!await _uow.Recipients.ExistsAsync(r => r.Id == rId))
                    throw new NotFoundException($"Recipient {rId} not found.");
            }

            // flowers наявність та кількість в складі
            foreach (var fq in dto.Flowers)
            {
                var flower = await _uow.Flowers.GetByIdAsync(fq.FlowerId);
                if (flower == null) throw new NotFoundException($"Flower {fq.FlowerId} not found.");
                if (flower.Quantity < fq.Quantity)
                    throw new Exception($"Not enough flower '{flower.Name}' in stock. Requested {fq.Quantity}, available {flower.Quantity}.");
            }

            // Gift optional
            if (dto.GiftId.HasValue)
            {
                if (!await _uow.Gifts.ExistsAsync(g => g.Id == dto.GiftId.Value))
                    throw new NotFoundException($"Gift {dto.GiftId.Value} not found.");
            }

            // create bouquet entity
            var bouquet = new Bouquet
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                // MainPhoto will be set after uploading
            };

            // main photo
            if (dto.MainPhoto != null)
            {
                using var ms = new MemoryStream();
                await dto.MainPhoto.CopyToAsync(ms);
                var mainUrl = await _imageService.UploadAsync(ms.ToArray(), dto.MainPhoto.FileName, "bouquets");
                bouquet.MainPhotoUrl = mainUrl;
            }

            await _uow.Bouquets.AddAsync(bouquet);
            // save to get Id
            await _uow.SaveChangesAsync();

            // sizes (many-to-many) — прив'язка
            var bouquetSize = new BouquetSize { BouquetId = bouquet.Id, SizeId = size.Id };
            // directly add to context set through repo pattern? we can use DbContext via UoW or repo; for simplicity, create in DbContext via repos not supported for join tables — but we can add to bouquet collection
            bouquet.BouquetSizes.Add(bouquetSize);

            // bouquet flowers
            foreach (var fq in dto.Flowers)
            {
                var bf = new BouquetFlower
                {
                    BouquetId = bouquet.Id,
                    FlowerId = fq.FlowerId,
                    Quantity = fq.Quantity
                };
                bouquet.BouquetFlowers.Add(bf);

                // optionally reduce flower stock or leave for "UpdateStock" request — depending on business logic
                var flower = await _uow.Flowers.GetByIdAsync(fq.FlowerId);
                flower.Quantity -= fq.Quantity;
                _uow.Flowers.Update(flower);
            }

            // events
            foreach (var evId in dto.EventIds)
            {
                bouquet.BouquetEvents.Add(new BouquetEvent { BouquetId = bouquet.Id, EventId = evId });
            }

            // recipients
            foreach (var rId in dto.RecipientIds)
            {
                bouquet.BouquetRecipients.Add(new BouquetRecipient { BouquetId = bouquet.Id, RecipientId = rId });
            }

            // gift
            if (dto.GiftId.HasValue)
            {
                bouquet.BouquetGifts.Add(new BouquetGift { BouquetId = bouquet.Id, GiftId = dto.GiftId.Value });
            }

            // images
            short pos = 1;
            foreach (var img in dto.Images.Take(3)) // максимум 3 фото
            {
                using var ms = new MemoryStream();
                await img.CopyToAsync(ms);

                var url = await _imageService.UploadAsync(ms.ToArray(), img.FileName, "bouquets");

                bouquet.BouquetImages.Add(new BouquetImage
                {
                    BouquetId = bouquet.Id,
                    ImageUrl = url,
                    Position = pos
                });

                pos++;
            }

            // persist changes
            _uow.Bouquets.Update(bouquet); // attach changes
            await _uow.SaveChangesAsync();

            return _mapper.Map<BouquetDto>(bouquet);
        }

        public async Task<BouquetDto> UpdateAsync(BouquetUpdateDto dto)
        {
            var bouquet = await _uow.Bouquets.GetByIdAsync(dto.Id);
            if (bouquet == null) throw new NotFoundException($"Bouquet {dto.Id} not found.");

            if (bouquet.Name != dto.Name && await _uow.Bouquets.ExistsAsync(b => b.Name == dto.Name))
                throw new AlreadyExistsException($"Bouquet with name '{dto.Name}' already exists.");

            bouquet.Name = dto.Name;
            bouquet.Description = dto.Description;
            bouquet.Price = dto.Price;

            // size
            var size = await _uow.Sizes.GetByIdAsync(dto.SizeId);
            if (size == null) throw new NotFoundException($"Size {dto.SizeId} not found.");
            // clear existing sizes and add new
            bouquet.BouquetSizes.Clear();
            bouquet.BouquetSizes.Add(new BouquetSize { BouquetId = bouquet.Id, SizeId = dto.SizeId });

            // gift
            if (dto.GiftId.HasValue)
            {
                if (!await _uow.Gifts.ExistsAsync(g => g.Id == dto.GiftId.Value))
                    throw new NotFoundException($"Gift {dto.GiftId} not found.");
                bouquet.BouquetGifts.Clear();
                bouquet.BouquetGifts.Add(new BouquetGift { BouquetId = bouquet.Id, GiftId = dto.GiftId.Value });
            }
            else
            {
                bouquet.BouquetGifts.Clear();
            }

            // flowers: replace existing bouquet_flowers
            bouquet.BouquetFlowers.Clear();
            foreach (var fq in dto.Flowers)
            {
                var flower = await _uow.Flowers.GetByIdAsync(fq.FlowerId);
                if (flower == null) throw new NotFoundException($"Flower {fq.FlowerId} not found.");
                bouquet.BouquetFlowers.Add(new BouquetFlower { BouquetId = bouquet.Id, FlowerId = fq.FlowerId, Quantity = fq.Quantity });
            }

            // events
            bouquet.BouquetEvents.Clear();
            foreach (var evId in dto.EventIds)
            {
                if (!await _uow.Events.ExistsAsync(e => e.Id == evId))
                    throw new NotFoundException($"Event {evId} not found.");
                bouquet.BouquetEvents.Add(new BouquetEvent { BouquetId = bouquet.Id, EventId = evId });
            }

            // recipients
            bouquet.BouquetRecipients.Clear();
            foreach (var rId in dto.RecipientIds)
            {
                if (!await _uow.Recipients.ExistsAsync(r => r.Id == rId))
                    throw new NotFoundException($"Recipient {rId} not found.");
                bouquet.BouquetRecipients.Add(new BouquetRecipient { BouquetId = bouquet.Id, RecipientId = rId });
            }

            // main photo optional
            if (dto.MainPhoto != null)
            {
                var mainUrl = await _imageService.UploadAsync(dto.MainPhoto.Content, dto.MainPhoto.FileName, "bouquets");
                bouquet.MainPhotoUrl = mainUrl;
            }

            // new images (append, up to 3 total)
            var existingCount = bouquet.BouquetImages?.Count ?? 0;
            var remainingSlots = Math.Max(0, 3 - existingCount);
            short posStart = (short)(existingCount + 1);
            foreach (var img in dto.NewImages.Take(remainingSlots))
            {
                var url = await _imageService.UploadAsync(img.Content, img.FileName, "bouquets");
                bouquet.BouquetImages.Add(new BouquetImage { BouquetId = bouquet.Id, ImageUrl = url, Position = posStart });
                posStart++;
            }

            _uow.Bouquets.Update(bouquet);
            await _uow.SaveChangesAsync();

            return _mapper.Map<BouquetDto>(bouquet);
        }

        public async Task UpdateFlowerQuantityAsync(Guid flowerId, int quantity)
        {
            var flower = await _uow.Flowers.GetByIdAsync(flowerId);
            if (flower == null) throw new NotFoundException($"Flower {flowerId} not found.");
            if (quantity < 0) throw new ArgumentException("Quantity must be non-negative.", nameof(quantity));
            flower.Quantity = quantity;
            _uow.Flowers.Update(flower);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var bouquet = await _uow.Bouquets.GetByIdAsync(id);
            if (bouquet == null)
                throw new NotFoundException($"Bouquet with id {id} not found.");

            // Видаляємо усі join-таблиці, якщо потрібно (EF Core cascade може це зробити)
            _uow.Bouquets.Delete(bouquet);
            await _uow.SaveChangesAsync();
        }
    }
}
