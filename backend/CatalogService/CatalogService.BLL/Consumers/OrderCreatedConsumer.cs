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

            foreach (var item in orderEvent.Bouquets)
            {
                var bouquet = await unitOfWork.Bouquets.GetWithDetailsAsync(item.BouquetId);
                if (bouquet == null)
                {
                    logger.LogWarning("Bouquet with ID {BouquetId} not found", item.BouquetId);
                    continue;
                }

                foreach (var bf in bouquet.BouquetFlowers)
                {
                    int required = bf.Quantity * item.Count;

                    if (bf.Flower.Quantity < required)
                    {
                        var msg = $"Not enough flowers '{bf.Flower.Name}'. " +
                                  $"Requested {required}, available {bf.Flower.Quantity}.";
                        logger.LogError("Not enough flowers: {Msg}", msg);
                        throw new InvalidOperationException(msg);
                    }

                    bf.Flower.Quantity -= required;
                    bf.Flower.UpdatedAt = DateTime.UtcNow;

                    unitOfWork.Flowers.Update(bf.Flower);

                    logger.LogInformation("{Count} flowers '{FlowerName}' (ID: {FlowerId}) deducted", required, bf.Flower.Name, bf.Flower.Id);
                }
                await cacheInvalidation.InvalidateByIdAsync(bouquet.Id);
            }

            await unitOfWork.SaveChangesAsync();

            await eventLogService.MarkEventAsProcessedAsync(eventId);

            await cacheInvalidation.InvalidateAllAsync();

            logger.LogInformation("Order {OrderId} successfully processed", orderEvent.OrderId);
        }
    }
}
