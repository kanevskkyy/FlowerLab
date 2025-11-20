using AutoMapper;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Exceptions;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.DAL.Helpers;
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using CatalogService.Domain.QueryParametrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit; 
using FlowerLab.Shared.Events;

namespace CatalogService.BLL.Services.Implementations
{
    public class BouquetService : IBouquetService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IPublishEndpoint _publishEndpoint;
        
        public BouquetService(IUnitOfWork uow, IMapper mapper, IImageService imageService, IPublishEndpoint publishEndpoint)
        {
            _uow = uow;
            _mapper = mapper;
            _imageService = imageService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<PagedList<BouquetDto>> GetAllAsync(BouquetQueryParameters parameters)
        {
            var pagedBouquets = await _uow.Bouquets.GetBySpecificationPagedAsync(parameters);

            var mapped = pagedBouquets.Items
                .Select(b => _mapper.Map<BouquetDto>(b))
                .ToList();

            return new PagedList<BouquetDto>(
                mapped,
                pagedBouquets.TotalCount,
                pagedBouquets.CurrentPage,
                pagedBouquets.PageSize
            );
        }

        public async Task<BouquetDto> GetByIdAsync(Guid id)
        {
            var bouquet = await _uow.Bouquets.GetWithDetailsAsync(id);
            if (bouquet == null) throw new NotFoundException($"Букет {id} не знайдено.");
            return _mapper.Map<BouquetDto>(bouquet);
        }

        public async Task<BouquetDto> CreateAsync(BouquetCreateDto dto)
        {
            if (await _uow.Bouquets.ExistsAsync(b => b.Name == dto.Name))
                throw new AlreadyExistsException($"Букет з назвою '{dto.Name}' уже існує.");

            var size = await _uow.Sizes.GetByIdAsync(dto.SizeId);
            if (size == null) throw new NotFoundException($"Розмір {dto.SizeId} не знайдено.");

            foreach (var evId in dto.EventIds)
                if (!await _uow.Events.ExistsAsync(e => e.Id == evId))
                    throw new NotFoundException($"Подія {evId} не знайдена.");

            foreach (var rId in dto.RecipientIds)
                if (!await _uow.Recipients.ExistsAsync(r => r.Id == rId))
                    throw new NotFoundException($"Отримувач {rId} не знайдений.");

            var flowers = dto.FlowerIds
                .Select((id, index) => new FlowerQuantityDto { FlowerId = id, Quantity = dto.FlowerQuantities[index] })
                .ToList();

            foreach (var fq in flowers)
            {
                var flower = await _uow.Flowers.GetByIdAsync(fq.FlowerId);
                if (flower == null) throw new NotFoundException($"Квітка {fq.FlowerId} не знайдена.");
                if (flower.Quantity < fq.Quantity)
                    throw new Exception($"Недостатньо квіток '{flower.Name}' на складі. Запитано {fq.Quantity}, доступно {flower.Quantity}.");
            }

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

            if (dto.MainPhoto != null)
            {
                using var ms = new MemoryStream();
                await dto.MainPhoto.CopyToAsync(ms);
                var mainUrl = await _imageService.UploadAsync(ms.ToArray(), dto.MainPhoto.FileName, "bouquets");
                bouquet.MainPhotoUrl = mainUrl;
            }

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

            foreach (var evId in dto.EventIds)
                bouquet.BouquetEvents.Add(new BouquetEvent { Bouquet = bouquet, EventId = evId });

            foreach (var rId in dto.RecipientIds)
                bouquet.BouquetRecipients.Add(new BouquetRecipient { Bouquet = bouquet, RecipientId = rId });

            short pos = 1;
            foreach (var img in dto.Images.Take(3))
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

            await _uow.Bouquets.AddAsync(bouquet);
            await _uow.SaveChangesAsync();

            var savedBouquet = await _uow.Bouquets.GetWithDetailsAsync(bouquet.Id);

            return _mapper.Map<BouquetDto>(savedBouquet);
        }

        public async Task<BouquetDto> UpdateAsync(Guid id, BouquetUpdateDto dto)
        {
            var bouquet = await _uow.Bouquets.GetWithDetailsAsync(id);
            if (bouquet == null) throw new NotFoundException($"Букет {id} не знайдено.");

            if (bouquet.Name != dto.Name && await _uow.Bouquets.ExistsAsync(b => b.Name == dto.Name))
                throw new AlreadyExistsException($"Букет з назвою '{dto.Name}' уже існує.");

            bouquet.Name = dto.Name;
            bouquet.Description = dto.Description;
            bouquet.Price = dto.Price;

            var size = await _uow.Sizes.GetByIdAsync(dto.SizeId);
            if (size == null) throw new NotFoundException($"Розмір {dto.SizeId} не знайдено.");
            bouquet.BouquetSizes.Clear();
            bouquet.BouquetSizes.Add(new BouquetSize { BouquetId = bouquet.Id, SizeId = dto.SizeId });

            var flowers = dto.FlowerIds
                .Select((id, index) => new FlowerQuantityDto { FlowerId = id, Quantity = dto.FlowerQuantities[index] })
                .ToList();

            foreach (var fq in flowers)
            {
                var flower = await _uow.Flowers.GetByIdAsync(fq.FlowerId);
                if (flower == null) throw new NotFoundException($"Квітка {fq.FlowerId} не знайдена.");
                if (flower.Quantity < fq.Quantity)
                    throw new Exception($"Недостатньо квіток '{flower.Name}' на складі. Запитано {fq.Quantity}, доступно {flower.Quantity}.");
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

            bouquet.BouquetEvents.Clear();
            foreach (var evId in dto.EventIds)
            {
                if (!await _uow.Events.ExistsAsync(e => e.Id == evId))
                    throw new NotFoundException($"Подія {evId} не знайдена.");
                bouquet.BouquetEvents.Add(new BouquetEvent { BouquetId = bouquet.Id, EventId = evId });
            }

            bouquet.BouquetRecipients.Clear();
            foreach (var rId in dto.RecipientIds)
            {
                if (!await _uow.Recipients.ExistsAsync(r => r.Id == rId))
                    throw new NotFoundException($"Отримувач {rId} не знайдений.");
                bouquet.BouquetRecipients.Add(new BouquetRecipient { BouquetId = bouquet.Id, RecipientId = rId });
            }

            if (dto.MainPhoto != null)
            {
                using var ms = new MemoryStream();
                await dto.MainPhoto.CopyToAsync(ms);
                var mainUrl = await _imageService.UploadAsync(ms.ToArray(), dto.MainPhoto.FileName, "bouquets");
                bouquet.MainPhotoUrl = mainUrl;
            }

            short pos = 1;
            foreach (var img in dto.NewImages.Take(3))
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
            if (flower == null) throw new NotFoundException($"Квітка {flowerId} не знайдена.");
            if (quantity < 0) throw new ArgumentException("Кількість повинна бути невід’ємною.", nameof(quantity));
            flower.Quantity = quantity;
            _uow.Flowers.Update(flower);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var bouquet = await _uow.Bouquets.GetWithDetailsAsync(id);
            if (bouquet == null)
                throw new NotFoundException($"Букет з ID {id} не знайдено.");

            _uow.Bouquets.Delete(bouquet);
            await _uow.SaveChangesAsync();
            
            await _publishEndpoint.Publish(new BouquetDeletedEvent(id));
        }
    }

}
