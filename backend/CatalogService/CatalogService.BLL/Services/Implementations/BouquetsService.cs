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
            // 1. Перевірка унікальності
            if (await _uow.Bouquets.ExistsAsync(b => b.Name == dto.Name))
                throw new AlreadyExistsException($"Bouquet with name '{dto.Name}' already exists.");

            // 2. Перевірка існування Size
            var size = await _uow.Sizes.GetByIdAsync(dto.SizeId);
            if (size == null) throw new NotFoundException($"Size {dto.SizeId} not found.");

            // 3. Перевірка Events
            foreach (var evId in dto.EventIds)
                if (!await _uow.Events.ExistsAsync(e => e.Id == evId))
                    throw new NotFoundException($"Event {evId} not found.");

            // 4. Перевірка Recipients
            foreach (var rId in dto.RecipientIds)
                if (!await _uow.Recipients.ExistsAsync(r => r.Id == rId))
                    throw new NotFoundException($"Recipient {rId} not found.");

            var flowers = dto.FlowerIds
    .Select((id, index) => new FlowerQuantityDto { FlowerId = id, Quantity = dto.FlowerQuantities[index] })
    .ToList();

            // 5. Перевірка Flowers (наявність та кількість)
            foreach (var fq in flowers)
            {
                var flower = await _uow.Flowers.GetByIdAsync(fq.FlowerId);
                if (flower == null) throw new NotFoundException($"Flower {fq.FlowerId} not found.");
                if (flower.Quantity < fq.Quantity)
                    throw new Exception($"Not enough flower '{flower.Name}' in stock. Requested {fq.Quantity}, available {flower.Quantity}.");
            }


            // 7. Створення Bouquet
            var bouquet = new Bouquet
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                BouquetSizes = new List<BouquetSize> { new BouquetSize { SizeId = dto.SizeId } },
                BouquetFlowers = new List<BouquetFlower>(),
                BouquetEvents = new List<BouquetEvent>(),
                BouquetRecipients = new List<BouquetRecipient>(),
                BouquetImages = new List<BouquetImage>()
            };

            // main photo
            if (dto.MainPhoto != null)
            {
                using var ms = new MemoryStream();
                await dto.MainPhoto.CopyToAsync(ms);
                var mainUrl = await _imageService.UploadAsync(ms.ToArray(), dto.MainPhoto.FileName, "bouquets");
                bouquet.MainPhotoUrl = mainUrl;
            }
            
            // 9. Flowers + зменшення складу
            foreach (var fq in flowers)
            {
 

                var bouquetFlower = new BouquetFlower
                {
                    Bouquet = bouquet,     
                    FlowerId = fq.FlowerId,
                    Quantity = fq.Quantity
                };

                bouquet.BouquetFlowers.Add(bouquetFlower);


            }

            // 10. Events
            foreach (var evId in dto.EventIds)
                bouquet.BouquetEvents.Add(new BouquetEvent { Bouquet = bouquet, EventId = evId });

            // 11. Recipients
            foreach (var rId in dto.RecipientIds)
                bouquet.BouquetRecipients.Add(new BouquetRecipient { Bouquet = bouquet, RecipientId = rId });

            
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

            // 14. Додаємо bouquet у репозиторій
            await _uow.Bouquets.AddAsync(bouquet);
            await _uow.SaveChangesAsync(); // збереження всіх зв’язків

            var savedBouquet = await _uow.Bouquets.GetByIdAsync(bouquet.Id);

            return _mapper.Map<BouquetDto>(savedBouquet);

        }

        public async Task<BouquetDto> UpdateAsync(Guid id, BouquetUpdateDto dto)
        {
            var bouquet = await _uow.Bouquets.GetByIdAsync(id);
            if (bouquet == null) throw new NotFoundException($"Bouquet {id} not found.");

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


            // flowers: replace existing bouquet_flowers
            var flowers = dto.FlowerIds
        .Select((id, index) => new FlowerQuantityDto { FlowerId = id, Quantity = dto.FlowerQuantities[index] })
        .ToList();

            // Перевірка наявності та кількості квітів у складі
            foreach (var fq in flowers)
            {
                var flower = await _uow.Flowers.GetByIdAsync(fq.FlowerId);
                if (flower == null) throw new NotFoundException($"Flower {fq.FlowerId} not found.");
                if (flower.Quantity < fq.Quantity)
                    throw new Exception($"Not enough flower '{flower.Name}' in stock. Requested {fq.Quantity}, available {flower.Quantity}.");
            }

            bouquet.BouquetFlowers.Clear();
            foreach (var fq in flowers)
            {
                bouquet.BouquetFlowers.Add(new BouquetFlower
                {
                    BouquetId = bouquet.Id,
                    FlowerId = fq.FlowerId,
                    Quantity = fq.Quantity
                });
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
                using var ms = new MemoryStream();
                await dto.MainPhoto.CopyToAsync(ms);
                var mainUrl = await _imageService.UploadAsync(ms.ToArray(), dto.MainPhoto.FileName, "bouquets");
                bouquet.MainPhotoUrl = mainUrl;
            }

            // new images (append, up to 3 total)
            short pos = 1;
            foreach (var img in dto.NewImages.Take(3)) // максимум 3 фото
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
