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

        public async Task<PagedList<BouquetSummaryDto>> GetAllAsync(BouquetQueryParameters parameters)
        {
            var pagedBouquets = await _uow.Bouquets.GetBySpecificationPagedAsync(parameters);

            var mapped = pagedBouquets.Items
                .Select(b => _mapper.Map<BouquetSummaryDto>(b))
                .ToList();

            return new PagedList<BouquetSummaryDto>(
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

            if (!dto.Sizes.Any())
                throw new Exception("Необхідно вказати хоча б один розмір.");

            foreach (var sizeDto in dto.Sizes)
            {
                var size = await _uow.Sizes.GetByIdAsync(sizeDto.SizeId);
                if (size == null)
                    throw new NotFoundException($"Розмір {sizeDto.SizeId} не знайдено.");

                if (sizeDto.FlowerIds.Count != sizeDto.FlowerQuantities.Count)
                    throw new Exception($"Кількість ID квіток не відповідає кількості для розміру {size.Name}.");

                if (!sizeDto.FlowerIds.Any())
                    throw new Exception($"Необхідно вказати квіти для розміру {size.Name}.");
            }

            foreach (var evId in dto.EventIds)
            {
                if (!await _uow.Events.ExistsAsync(e => e.Id == evId))
                    throw new NotFoundException($"Подія {evId} не знайдена.");
            }

            foreach (var rId in dto.RecipientIds)
            {
                if (!await _uow.Recipients.ExistsAsync(r => r.Id == rId))
                    throw new NotFoundException($"Отримувач {rId} не знайдений.");
            }

            foreach (var sizeDto in dto.Sizes)
            {
                for (int i = 0; i < sizeDto.FlowerIds.Count; i++)
                {
                    var flower = await _uow.Flowers.GetByIdAsync(sizeDto.FlowerIds[i]);
                    if (flower == null)
                        throw new NotFoundException($"Квітка {sizeDto.FlowerIds[i]} не знайдена.");

                    if (flower.Quantity < sizeDto.FlowerQuantities[i])
                    {
                        var size = await _uow.Sizes.GetByIdAsync(sizeDto.SizeId);
                        throw new Exception($"Недостатньо квіток '{flower.Name}' для розміру {size.Name}. " +
                            $"Запитано {sizeDto.FlowerQuantities[i]}, доступно {flower.Quantity}.");
                    }
                }
            }

            var bouquet = new Bouquet
            {
                Name = dto.Name,
                Description = dto.Description,
                MainPhotoUrl = string.Empty, 
                BouquetSizes = new List<BouquetSize>(),
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
            else
            {
                throw new Exception("Головне фото є обов'язковим.");
            }

            foreach (var sizeDto in dto.Sizes)
            {
                var bouquetSize = new BouquetSize
                {
                    BouquetId = bouquet.Id,
                    SizeId = sizeDto.SizeId,
                    Price = sizeDto.Price,
                    BouquetSizeFlowers = new List<BouquetSizeFlower>()
                };

                for (int i = 0; i < sizeDto.FlowerIds.Count; i++)
                {
                    var bouquetSizeFlower = new BouquetSizeFlower
                    {
                        BouquetId = bouquet.Id,
                        SizeId = sizeDto.SizeId,
                        FlowerId = sizeDto.FlowerIds[i],
                        Quantity = sizeDto.FlowerQuantities[i]
                    };
                    bouquetSize.BouquetSizeFlowers.Add(bouquetSizeFlower);
                }

                bouquet.BouquetSizes.Add(bouquetSize);
            }

            foreach (var evId in dto.EventIds)
            {
                bouquet.BouquetEvents.Add(new BouquetEvent
                {
                    BouquetId = bouquet.Id,
                    EventId = evId
                });
            }

            foreach (var rId in dto.RecipientIds)
            {
                bouquet.BouquetRecipients.Add(new BouquetRecipient
                {
                    BouquetId = bouquet.Id,
                    RecipientId = rId
                });
            }

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
            if (bouquet == null)
                throw new NotFoundException($"Букет {id} не знайдено.");

            if (bouquet.Name != dto.Name && await _uow.Bouquets.ExistsAsync(b => b.Name == dto.Name))
                throw new AlreadyExistsException($"Букет з назвою '{dto.Name}' уже існує.");

            if (!dto.Sizes.Any())
                throw new Exception("Необхідно вказати хоча б один розмір.");

            foreach (var sizeDto in dto.Sizes)
            {
                var size = await _uow.Sizes.GetByIdAsync(sizeDto.SizeId);
                if (size == null)
                    throw new NotFoundException($"Розмір {sizeDto.SizeId} не знайдено.");

                if (sizeDto.FlowerIds.Count != sizeDto.FlowerQuantities.Count)
                    throw new Exception($"Кількість ID квіток не відповідає кількості для розміру {size.Name}.");

                if (!sizeDto.FlowerIds.Any())
                    throw new Exception($"Необхідно вказати квіти для розміру {size.Name}.");
            }

            foreach (var evId in dto.EventIds)
            {
                if (!await _uow.Events.ExistsAsync(e => e.Id == evId))
                    throw new NotFoundException($"Подія {evId} не знайдена.");
            }

            foreach (var rId in dto.RecipientIds)
            {
                if (!await _uow.Recipients.ExistsAsync(r => r.Id == rId))
                    throw new NotFoundException($"Отримувач {rId} не знайдений.");
            }

            foreach (var sizeDto in dto.Sizes)
            {
                for (int i = 0; i < sizeDto.FlowerIds.Count; i++)
                {
                    var flower = await _uow.Flowers.GetByIdAsync(sizeDto.FlowerIds[i]);
                    if (flower == null)
                        throw new NotFoundException($"Квітка {sizeDto.FlowerIds[i]} не знайдена.");

                    if (flower.Quantity < sizeDto.FlowerQuantities[i])
                    {
                        var size = await _uow.Sizes.GetByIdAsync(sizeDto.SizeId);
                        throw new Exception($"Недостатньо квіток '{flower.Name}' для розміру {size.Name}. " +
                            $"Запитано {sizeDto.FlowerQuantities[i]}, доступно {flower.Quantity}.");
                    }
                }
            }

            bouquet.Name = dto.Name;
            bouquet.Description = dto.Description;

            bouquet.BouquetSizes.Clear();
            foreach (var sizeDto in dto.Sizes)
            {
                var bouquetSize = new BouquetSize
                {
                    BouquetId = bouquet.Id,
                    Price = sizeDto.Price,
                    SizeId = sizeDto.SizeId,
                    BouquetSizeFlowers = new List<BouquetSizeFlower>()
                };

                for (int i = 0; i < sizeDto.FlowerIds.Count; i++)
                {
                    bouquetSize.BouquetSizeFlowers.Add(new BouquetSizeFlower
                    {
                        BouquetId = bouquet.Id,
                        SizeId = sizeDto.SizeId,
                        FlowerId = sizeDto.FlowerIds[i],
                        Quantity = sizeDto.FlowerQuantities[i]
                    });
                }

                bouquet.BouquetSizes.Add(bouquetSize);
            }

            bouquet.BouquetEvents.Clear();
            foreach (var evId in dto.EventIds)
            {
                bouquet.BouquetEvents.Add(new BouquetEvent
                {
                    BouquetId = bouquet.Id,
                    EventId = evId
                });
            }

            bouquet.BouquetRecipients.Clear();
            foreach (var rId in dto.RecipientIds)
            {
                bouquet.BouquetRecipients.Add(new BouquetRecipient
                {
                    BouquetId = bouquet.Id,
                    RecipientId = rId
                });
            }

            if (dto.MainPhoto != null)
            {
                using var ms = new MemoryStream();
                await dto.MainPhoto.CopyToAsync(ms);
                var mainUrl = await _imageService.UploadAsync(ms.ToArray(), dto.MainPhoto.FileName, "bouquets");
                bouquet.MainPhotoUrl = mainUrl;
            }

            if (dto.NewImages != null && dto.NewImages.Any())
            {
                bouquet.BouquetImages.Clear();

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
            }

            _uow.Bouquets.Update(bouquet);
            await _uow.SaveChangesAsync();

            var updatedBouquet = await _uow.Bouquets.GetWithDetailsAsync(bouquet.Id);
            return _mapper.Map<BouquetDto>(updatedBouquet);
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

            Console.WriteLine($"[CATALOG] Букет {id} видалено з БД");
            Console.WriteLine($"[CATALOG] Публікую BouquetDeletedEvent для ID: {id}");

            try
            {
                await _publishEndpoint.Publish(new BouquetDeletedEvent(id));
                Console.WriteLine($"[CATALOG] ✓ Event успішно опубліковано для ID: {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CATALOG] ✗ Помилка публікації event: {ex.Message}");
                throw;
            }
        }
    }

}
