using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using shared.cache;
using shared.events.EventService;
using shared.events.OrderEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.BLL.Consumers
{
    public class OrderCancelledConsumer : IConsumer<OrderCancelledEvent>
    {
        private IUnitOfWork unitOfWork;
        private ILogger<OrderCancelledConsumer> logger;
        private IEventLogService eventLogService;
        private IEntityCacheInvalidationService<Bouquet> cacheInvalidation;

        public OrderCancelledConsumer(
            IUnitOfWork unitOfWork,
            ILogger<OrderCancelledConsumer> logger,
            IEventLogService eventLogService,
            IEntityCacheInvalidationService<Bouquet> cacheInvalidation)
        {
            this.cacheInvalidation = cacheInvalidation;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.eventLogService = eventLogService;
        }

        public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
        {
            var eventId = context.Message.EventId;

            if (await eventLogService.HasEventProcessedAsync(eventId))
            {
                logger.LogWarning("Event {EventId} has already been processed. Skipping…", eventId);
                return;
            }

            var orderEvent = context.Message;
            logger.LogInformation("Processing cancelled order {OrderId}", orderEvent.OrderId);

            var flowerReturns = new Dictionary<Guid, int>();
            var bouquetIdsToInvalidate = new HashSet<Guid>();

            foreach (var item in orderEvent.Bouquets)
            {
                var bouquet = await unitOfWork.Bouquets.GetWithDetailsAsync(item.BouquetId);
                if (bouquet == null)
                {
                    logger.LogWarning("Bouquet with ID {BouquetId} not found", item.BouquetId);
                    continue;
                }

                bouquetIdsToInvalidate.Add(bouquet.Id);

                var size = bouquet.BouquetSizes.FirstOrDefault(s => s.SizeId == item.SizeId);
                if (size == null)
                {
                    logger.LogWarning("Size {SizeId} for Bouquet {BouquetId} not found", item.SizeId, item.BouquetId);
                    continue;
                }

                foreach (var bsf in size.BouquetSizeFlowers)
                {
                    int totalReturned = bsf.Quantity * item.Count;
                    if (flowerReturns.TryGetValue(bsf.FlowerId, out int current))
                    {
                        flowerReturns[bsf.FlowerId] = current + totalReturned;
                    }
                    else
                    {
                        flowerReturns[bsf.FlowerId] = totalReturned;
                    }
                }
            }

            foreach (var ret in flowerReturns)
            {
                var flower = await unitOfWork.Flowers.GetByIdTrackedAsync(ret.Key); 
                
                if (flower == null)
                {
                    logger.LogWarning("Flower {FlowerId} not found during restore", ret.Key);
                    continue;
                }

                flower.Quantity += ret.Value;
                flower.UpdatedAt = DateTime.UtcNow;

                unitOfWork.Flowers.Update(flower);

                logger.LogInformation("{Count} flowers '{FlowerName}' (ID: {FlowerId}) restored", ret.Value, flower.Name, flower.Id);
            }

            foreach (var bouquetId in bouquetIdsToInvalidate)
            {
                await cacheInvalidation.InvalidateByIdAsync(bouquetId);
            }

            await unitOfWork.SaveChangesAsync();

            await eventLogService.MarkEventAsProcessedAsync(eventId);

            await cacheInvalidation.InvalidateAllAsync();

            logger.LogInformation("Order {OrderId} cancellation successfully processed", orderEvent.OrderId);
        }
    }
}
