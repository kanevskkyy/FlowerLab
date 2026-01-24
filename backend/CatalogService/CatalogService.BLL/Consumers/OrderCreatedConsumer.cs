using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using shared.cache;
using shared.events;
using shared.events.EventService;
using System;
using System.Threading.Tasks;

namespace CatalogService.BLL.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private IUnitOfWork unitOfWork;
        private ILogger<OrderCreatedConsumer> logger;
        private IEventLogService eventLogService;
        private IEntityCacheInvalidationService<Bouquet> cacheInvalidation;

        public OrderCreatedConsumer(
            IUnitOfWork unitOfWork,
            ILogger<OrderCreatedConsumer> logger,
            IEventLogService eventLogService,
            IEntityCacheInvalidationService<Bouquet> cacheInvalidation)
        {
            this.cacheInvalidation = cacheInvalidation;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.eventLogService = eventLogService;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var eventId = context.Message.EventId;

            if (await eventLogService.HasEventProcessedAsync(eventId))
            {
                logger.LogWarning("Event {EventId} has already been processed. Skipping…", eventId);
                return;
            }

            var orderEvent = context.Message;
            logger.LogInformation("Processing new order {OrderId}", orderEvent.OrderId);

            var flowerRequirements = new Dictionary<Guid, int>();
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
                    int totalRequired = bsf.Quantity * item.Count;
                    if (flowerRequirements.TryGetValue(bsf.FlowerId, out int current))
                    {
                        flowerRequirements[bsf.FlowerId] = current + totalRequired;
                    }
                    else
                    {
                        flowerRequirements[bsf.FlowerId] = totalRequired;
                    }
                }
            }

            foreach (var req in flowerRequirements)
            {
                
                var flower = await unitOfWork.Flowers.GetByIdTrackedAsync(req.Key); 
                
                if (flower == null)
                {
                    logger.LogWarning("Flower {FlowerId} not found during deduction", req.Key);
                    continue;
                }

                if (flower.Quantity < req.Value)
                {
                    var msg = $"Not enough flowers '{flower.Name}'. Requested {req.Value}, available {flower.Quantity}.";
                    logger.LogError("Not enough flowers: {Msg}", msg);
                    throw new InvalidOperationException(msg);
                }

                flower.Quantity -= req.Value;
                flower.UpdatedAt = DateTime.UtcNow;

                unitOfWork.Flowers.Update(flower);

                logger.LogInformation("{Count} flowers '{FlowerName}' (ID: {FlowerId}) deducted", req.Value, flower.Name, flower.Id);
            }

            foreach (var bouquetId in bouquetIdsToInvalidate)
            {
                await cacheInvalidation.InvalidateByIdAsync(bouquetId);
            }

            await unitOfWork.SaveChangesAsync();

            await eventLogService.MarkEventAsProcessedAsync(eventId);

            await cacheInvalidation.InvalidateAllAsync();

            logger.LogInformation("Order {OrderId} successfully processed", orderEvent.OrderId);
        }
    }
}
