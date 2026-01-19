using AutoMapper;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Exceptions;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.DAL.Helpers;
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using CatalogService.Domain.QueryParametrs;
using FlowerLab.Shared.Events;
using MassTransit; 
using shared.cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Implementations
{
    public class BouquetService : IBouquetService
    {
        private IUnitOfWork uow;
        private IMapper mapper;
        private IImageService imageService;
        private IPublishEndpoint publishEndpoint;
        private IEntityCacheService cache;
        private IEntityCacheInvalidationService<Bouquet> cacheInvalidation;
        private IEntityCacheInvalidationService<FilterResponse> filterCacheInvalidationService;

        public BouquetService(
            IUnitOfWork uow,
            IMapper mapper,
            IImageService imageService,
            IPublishEndpoint publishEndpoint,
            IEntityCacheService cache,
            IEntityCacheInvalidationService<Bouquet> cacheInvalidation,
            IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.imageService = imageService;
            this.publishEndpoint = publishEndpoint;

            this.cache = cache;
            this.cacheInvalidation = cacheInvalidation;
            this.filterCacheInvalidationService = entityCacheInvalidationService;
        }

        public async Task<PagedList<BouquetSummaryDto>?> GetAllAsync(BouquetQueryParameters parameters,
            CancellationToken cancellationToken = default)
        {
            return await cache.GetOrSetAsync(
                parameters.ToCacheKey(),
                async () => await FetchBouquetsAsync(parameters),
                memoryExpiration: TimeSpan.FromSeconds(30),
                redisExpiration: TimeSpan.FromMinutes(5)
            );
        }

        private async Task<PagedList<BouquetSummaryDto>> FetchBouquetsAsync(BouquetQueryParameters parameters,
            CancellationToken cancellationToken = default)
        {
            PagedList<Bouquet> pagedBouquets =
                await uow.Bouquets.GetBySpecificationPagedAsync(parameters, cancellationToken);
            List<BouquetSummaryDto> mapped = pagedBouquets.Items.Select(b => mapper.Map<BouquetSummaryDto>(b)).ToList();
            return PagedList<BouquetSummaryDto>.Create(
                mapped,
                pagedBouquets.TotalCount,
                pagedBouquets.CurrentPage,
                pagedBouquets.PageSize
            );
        }

        public async Task<BouquetDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"bouquet:{id}";

            return await cache.GetOrSetAsync(cacheKey, async () =>
            {
                Bouquet? bouquet = await uow.Bouquets.GetWithDetailsAsync(id, cancellationToken);
                if (bouquet == null) throw new NotFoundException($"Bouquet {id} not found.");

                return mapper.Map<BouquetDto>(bouquet);
            }, memoryExpiration: TimeSpan.FromSeconds(30), redisExpiration: TimeSpan.FromMinutes(5));
        }

        public async Task<BouquetDto> CreateAsync(BouquetCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (await uow.Bouquets.ExistsAsync(b => b.Name == dto.Name, cancellationToken))
            {
                throw new AlreadyExistsException($"Bouquet with name '{dto.Name}' already exists.");
            }

            if (!dto.Sizes.Any())
            {
                throw new BadRequestException("At least one size must be specified.");
            }

            if (dto.Sizes.GroupBy(s => s.SizeId).Any(g => g.Count() > 1))
            {
                throw new BadRequestException("Each size can only be specified once.");
            }

            foreach (var sizeDto in dto.Sizes)
            {
                Size? size = await uow.Sizes.GetByIdAsync(sizeDto.SizeId, cancellationToken);
                if (size == null)
                {
                    throw new NotFoundException($"Size {sizeDto.SizeId} not found.");
                }

                if (sizeDto.FlowerIds.Count != sizeDto.FlowerQuantities.Count)
                {
                    throw new BadRequestException(
                        $"Number of flower IDs does not match quantities for size {size.Name}.");
                }

                if (!sizeDto.FlowerIds.Any())
                {
                    throw new BadRequestException($"Flowers must be specified for size {size.Name}.");
                }
            }

            foreach (var evId in dto.EventIds)
            {
                if (!await uow.Events.ExistsAsync(e => e.Id == evId, cancellationToken))
                    throw new NotFoundException($"Event {evId} not found.");
            }

            foreach (var rId in dto.RecipientIds)
            {
                if (!await uow.Recipients.ExistsAsync(r => r.Id == rId, cancellationToken))
                    throw new NotFoundException($"Recipient {rId} not found.");
            }

            foreach (var sizeDto in dto.Sizes)
            {
                for (int i = 0; i < sizeDto.FlowerIds.Count; i++)
                {
                    Flower? flower = await uow.Flowers.GetByIdAsync(sizeDto.FlowerIds[i], cancellationToken);
                    if (flower == null)
                        throw new NotFoundException($"Flower {sizeDto.FlowerIds[i]} not found.");

                    if (flower.Quantity < sizeDto.FlowerQuantities[i])
                    {
                        Size? size = await uow.Sizes.GetByIdAsync(sizeDto.SizeId, cancellationToken);
                        throw new BadRequestException($"Not enough '{flower.Name}' flowers for size {size.Name}. " +
                                                      $"Requested {sizeDto.FlowerQuantities[i]}, available {flower.Quantity}.");
                    }
                }
            }

            Bouquet bouquet = new Bouquet
            {
                Name = dto.Name,
                Description = dto.Description,
                MainPhotoUrl = string.Empty,
                BouquetSizes = new List<BouquetSize>(),
                BouquetEvents = new List<BouquetEvent>(),
                BouquetRecipients = new List<BouquetRecipient>()
            };

            if (dto.MainPhoto != null)
            {
                using var ms = new MemoryStream();
                await dto.MainPhoto.CopyToAsync(ms);
                bouquet.MainPhotoUrl = await imageService.UploadAsync(ms.ToArray(), dto.MainPhoto.FileName, "bouquets");
            }
            else
            {
                throw new BadRequestException("Main photo is required.");
            }

            foreach (var sizeDto in dto.Sizes)
            {
                BouquetSize bouquetSize = new BouquetSize
                {
                    BouquetId = bouquet.Id,
                    SizeId = sizeDto.SizeId,
                    Price = sizeDto.Price,
                    BouquetSizeFlowers = new List<BouquetSizeFlower>(),
                    BouquetImages = new List<BouquetImage>()
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

                if (sizeDto.MainImage != null)
                {
                    using var ms = new MemoryStream();
                    await sizeDto.MainImage.CopyToAsync(ms);
                    string url = await imageService.UploadAsync(ms.ToArray(), sizeDto.MainImage.FileName, "bouquets");

                    bouquetSize.BouquetImages.Add(new BouquetImage
                    {
                        BouquetId = bouquet.Id,
                        SizeId = sizeDto.SizeId,
                        ImageUrl = url,
                        Position = 1,
                        IsMain = true,
                        Id = Guid.Empty // Force Added state
                    });
                }

                short position = 2;
                foreach (var img in sizeDto.AdditionalImages)
                {
                    using var ms = new MemoryStream();
                    await img.CopyToAsync(ms);
                    string url = await imageService.UploadAsync(ms.ToArray(), img.FileName, "bouquets");

                    bouquetSize.BouquetImages.Add(new BouquetImage
                    {
                        BouquetId = bouquet.Id,
                        SizeId = sizeDto.SizeId,
                        ImageUrl = url,
                        Position = position,
                        IsMain = false,
                        Id = Guid.Empty // Force Added state
                    });
                    position++;
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

            await uow.Bouquets.AddAsync(bouquet, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            await cacheInvalidation.InvalidateAllAsync();
            await filterCacheInvalidationService.InvalidateAllAsync();

            var savedBouquet = await uow.Bouquets.GetWithDetailsAsync(bouquet.Id, cancellationToken);
            return mapper.Map<BouquetDto>(savedBouquet);
        }

        public async Task<BouquetDto> UpdateAsync(Guid id, BouquetUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            Bouquet? bouquet = await uow.Bouquets.GetWithDetailsAsync(id, cancellationToken);
            if (bouquet == null)
                throw new NotFoundException($"Bouquet {id} not found.");

            if (bouquet.Name != dto.Name && await uow.Bouquets.ExistsAsync(b => b.Name == dto.Name, cancellationToken))
                throw new AlreadyExistsException($"Bouquet with name '{dto.Name}' already exists.");

            if (!dto.Sizes.Any())
                throw new BadRequestException("At least one size must be specified.");

            if (dto.Sizes.GroupBy(s => s.SizeId).Any(g => g.Count() > 1))
                throw new BadRequestException("Each size can only be specified once.");

            foreach (var sizeDto in dto.Sizes)
            {
                Size? size = await uow.Sizes.GetByIdAsync(sizeDto.SizeId, cancellationToken);
                if (size == null)
                    throw new NotFoundException($"Size {sizeDto.SizeId} not found.");

                if (sizeDto.FlowerIds.Count != sizeDto.FlowerQuantities.Count)
                    throw new BadRequestException(
                        $"Number of flower IDs does not match quantities for size {size.Name}.");

                if (!sizeDto.FlowerIds.Any())
                    throw new BadRequestException($"Flowers must be specified for size {size.Name}.");
            }

            foreach (var evId in dto.EventIds)
            {
                if (!await uow.Events.ExistsAsync(e => e.Id == evId, cancellationToken))
                    throw new NotFoundException($"Event {evId} not found.");
            }

            foreach (var rId in dto.RecipientIds)
            {
                if (!await uow.Recipients.ExistsAsync(r => r.Id == rId, cancellationToken))
                    throw new NotFoundException($"Recipient {rId} not found.");
            }

            foreach (var sizeDto in dto.Sizes)
            {
                for (int i = 0; i < sizeDto.FlowerIds.Count; i++)
                {
                    var flower = await uow.Flowers.GetByIdAsync(sizeDto.FlowerIds[i], cancellationToken);
                    if (flower == null)
                        throw new NotFoundException($"Flower {sizeDto.FlowerIds[i]} not found.");

                    if (flower.Quantity < sizeDto.FlowerQuantities[i])
                    {
                        var size = await uow.Sizes.GetByIdAsync(sizeDto.SizeId, cancellationToken);
                        throw new BadRequestException($"Not enough '{flower.Name}' flowers for size {size.Name}. " +
                                                      $"Requested {sizeDto.FlowerQuantities[i]}, available {flower.Quantity}.");
                    }
                }
            }

            bouquet.Name = dto.Name;
            bouquet.Description = dto.Description;

            if (dto.MainPhoto != null)
            {
                using var ms = new MemoryStream();
                await dto.MainPhoto.CopyToAsync(ms);
                bouquet.MainPhotoUrl = await imageService.UploadAsync(ms.ToArray(), dto.MainPhoto.FileName, "bouquets");
            }

            // 1. Sync Sizes
            var currentSizeIds = dto.Sizes.Select(s => s.SizeId).ToList();

            // Remove sizes not in DTO
            var sizesToRemove = bouquet.BouquetSizes.Where(bs => !currentSizeIds.Contains(bs.SizeId)).ToList();
            foreach (var sizeToRemove in sizesToRemove)
            {
                bouquet.BouquetSizes.Remove(sizeToRemove);
            }

            // Update or Add sizes
            foreach (var sizeDto in dto.Sizes)
            {
                var existingSize = bouquet.BouquetSizes.FirstOrDefault(bs => bs.SizeId == sizeDto.SizeId);

                if (existingSize != null)
                {
                    // UPDATE existing size
                    existingSize.Price = sizeDto.Price;

                    // Sync Flowers for this size
                    var currentFlowerIds = sizeDto.FlowerIds;

                    // Remove flowers
                    var flowersToRemove = existingSize.BouquetSizeFlowers
                        .Where(bsf => !currentFlowerIds.Contains(bsf.FlowerId)).ToList();
                    foreach (var f in flowersToRemove)
                    {
                        existingSize.BouquetSizeFlowers.Remove(f);
                    }

                    // Add or Update flowers
                    for (int i = 0; i < sizeDto.FlowerIds.Count; i++)
                    {
                        var fId = sizeDto.FlowerIds[i];
                        var qty = sizeDto.FlowerQuantities[i];
                        var existingFlower = existingSize.BouquetSizeFlowers.FirstOrDefault(f => f.FlowerId == fId);

                        if (existingFlower != null)
                        {
                            existingFlower.Quantity = qty;
                        }
                        else
                        {
                            existingSize.BouquetSizeFlowers.Add(new BouquetSizeFlower
                            {
                                BouquetId = bouquet.Id,
                                SizeId = sizeDto.SizeId,
                                FlowerId = fId,
                                Quantity = qty
                            });
                        }
                    }

                    try
                    {
                        if (sizeDto.AdditionalImages != null && sizeDto.AdditionalImages.Any())
                        {
                            var maxPos = existingSize.BouquetImages.Any()
                                ? existingSize.BouquetImages.Max(bi => bi.Position)
                                : (short)0;

                            foreach (var img in sizeDto.AdditionalImages)
                            {
                                using var ms = new MemoryStream();
                                await img.CopyToAsync(ms);
                                string url = await imageService.UploadAsync(ms.ToArray(), img.FileName, "bouquets");

                                var newImg = new BouquetImage
                                {
                                    BouquetId = bouquet.Id,
                                    SizeId = sizeDto.SizeId,
                                    ImageUrl = url,
                                    Position = ++maxPos,
                                    IsMain = false, // Always add as additional
                                    Id = Guid
                                        .Empty // IMPORTANT: Force "Added" state to override BaseEntity defaults
                                };
                                existingSize.BouquetImages.Add(newImg);
                                uow.Bouquets.AddImage(newImg); // Explicitly track as Added
                            }
                        }

                        if (sizeDto.ImageIdsToDelete != null && sizeDto.ImageIdsToDelete.Any())
                        {
                            var imagesToDelete = existingSize.BouquetImages
                                .Where(bi => sizeDto.ImageIdsToDelete.Contains(bi.Id))
                                .ToList();

                            foreach (var img in imagesToDelete)
                            {
                                existingSize.BouquetImages.Remove(img);
                                // Optional: Delete from cloud storage if needed
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[UpdateAsync] Error in image processing: {ex}");
                        throw new BadRequestException($"Image processing failed: {ex.Message}");
                    }
                }

                else
                {
                    // ADD New Size
                    var newSize = new BouquetSize
                    {
                        BouquetId = bouquet.Id,
                        SizeId = sizeDto.SizeId,
                        Price = sizeDto.Price,
                        BouquetSizeFlowers = new List<BouquetSizeFlower>(),
                        BouquetImages = new List<BouquetImage>()
                    };

                    for (int i = 0; i < sizeDto.FlowerIds.Count; i++)
                    {
                        newSize.BouquetSizeFlowers.Add(new BouquetSizeFlower
                        {
                            BouquetId = bouquet.Id,
                            SizeId = sizeDto.SizeId,
                            FlowerId = sizeDto.FlowerIds[i],
                            Quantity = sizeDto.FlowerQuantities[i]
                        });
                    }

                    if (sizeDto.MainImage != null)
                    {
                        using var ms = new MemoryStream();
                        await sizeDto.MainImage.CopyToAsync(ms);
                        string url = await imageService.UploadAsync(ms.ToArray(), sizeDto.MainImage.FileName,
                            "bouquets");

                        newSize.BouquetImages.Add(new BouquetImage
                        {
                            BouquetId = bouquet.Id,
                            SizeId = sizeDto.SizeId,
                            ImageUrl = url,
                            Position = 1,
                            IsMain = true,
                            Id = Guid.Empty // Force "Added" state
                        });
                    }

                    if (sizeDto.AdditionalImages != null && sizeDto.AdditionalImages.Any())
                    {
                        short position = 2;
                        foreach (var img in sizeDto.AdditionalImages)
                        {
                            using var ms = new MemoryStream();
                            await img.CopyToAsync(ms);
                            string url = await imageService.UploadAsync(ms.ToArray(), img.FileName, "bouquets");

                            newSize.BouquetImages.Add(new BouquetImage
                            {
                                BouquetId = bouquet.Id,
                                SizeId = sizeDto.SizeId,
                                ImageUrl = url,
                                Position = position++,
                                IsMain = false,
                                Id = Guid.Empty // Force "Added" state
                            });
                        }
                    }

                    bouquet.BouquetSizes.Add(newSize);
                }
            }

            // 2. Sync Events
            var eventsToRemove = bouquet.BouquetEvents.Where(be => !dto.EventIds.Contains(be.EventId)).ToList();
            foreach (var e in eventsToRemove) bouquet.BouquetEvents.Remove(e);

            foreach (var evId in dto.EventIds)
            {
                if (!bouquet.BouquetEvents.Any(be => be.EventId == evId))
                {
                    bouquet.BouquetEvents.Add(new BouquetEvent { BouquetId = bouquet.Id, EventId = evId });
                }
            }

            // 3. Sync Recipients
            var recipientsToRemove = bouquet.BouquetRecipients
                .Where(br => !dto.RecipientIds.Contains(br.RecipientId)).ToList();
            foreach (var r in recipientsToRemove) bouquet.BouquetRecipients.Remove(r);

            foreach (var rId in dto.RecipientIds)
            {
                if (!bouquet.BouquetRecipients.Any(br => br.RecipientId == rId))
                {
                    bouquet.BouquetRecipients.Add(
                        new BouquetRecipient { BouquetId = bouquet.Id, RecipientId = rId });
                }
            }

            uow.Bouquets.Update(bouquet);
            // DEFENSIVE FIX: Force all "Modified" BouquetImages to "Unchanged"
            // We strictly follow "Append-Only" for images. Valid existing images should NOT be updated.
            var modifiedImages = uow.GetChangeTrackerEntries()
                .Where(e => e.Entity is BouquetImage &&
                            e.State == Microsoft.EntityFrameworkCore.EntityState.Modified);

            foreach (var imgEntry in modifiedImages)
            {
                imgEntry.State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
            }

            await uow.SaveChangesAsync(cancellationToken);

            await cacheInvalidation.InvalidateAllAsync();
            await cacheInvalidation.InvalidateByIdAsync(bouquet.Id);
            await filterCacheInvalidationService.InvalidateAllAsync();

            var updatedBouquet = await uow.Bouquets.GetWithDetailsAsync(bouquet.Id, cancellationToken);
            return mapper.Map<BouquetDto>(updatedBouquet);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Bouquet? bouquet = await uow.Bouquets.GetWithDetailsAsync(id, cancellationToken);
            if (bouquet == null)
                throw new NotFoundException($"Bouquet with ID {id} not found.");

            uow.Bouquets.Delete(bouquet);
            await uow.SaveChangesAsync(cancellationToken);

            await cacheInvalidation.InvalidateAllAsync();
            await cacheInvalidation.InvalidateByIdAsync(bouquet.Id);
            await filterCacheInvalidationService.InvalidateAllAsync();

            await publishEndpoint.Publish(new BouquetDeletedEvent(id));
        }
    }
}
    

