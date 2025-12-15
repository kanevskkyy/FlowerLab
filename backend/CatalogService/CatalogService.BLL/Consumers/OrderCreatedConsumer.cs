using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using shared.cache;
using shared.events;

namespace CatalogService.BLL.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly IEventLogService _eventLogService;
        private readonly IEntityCacheInvalidationService<Bouquet> _cacheInvalidation;


        public OrderCreatedConsumer(
            IUnitOfWork unitOfWork, 
            ILogger<OrderCreatedConsumer> logger, 
            IEventLogService eventLogService,
            IEntityCacheInvalidationService<Bouquet> cacheInvalidation)
        {
            _cacheInvalidation = cacheInvalidation;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _eventLogService = eventLogService;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var eventId = context.Message.EventId;

            if (await _eventLogService.HasEventProcessedAsync(eventId))
            {
                _logger.LogWarning("Подія {EventId} вже оброблена. Пропуск…", eventId);
                return;
            }

            var orderEvent = context.Message;
            _logger.LogInformation("Обробка нового замовлення {OrderId}", orderEvent.OrderId);

            foreach (var item in orderEvent.Bouquets)
            {
                var bouquet = await _unitOfWork.Bouquets.GetWithDetailsAsync(item.BouquetId);
                if (bouquet == null)
                {
                    _logger.LogWarning("Букет з ID {BouquetId} не знайдено", item.BouquetId);
                    continue;
                }

                foreach (var bf in bouquet.BouquetFlowers)
                {
                    int required = bf.Quantity * item.Count;

                    if (bf.Flower.Quantity < required)
                    {
                        var msg = $"Недостатньо квітів '{bf.Flower.Name}'. " +
                                  $"Запитано {required}, доступно {bf.Flower.Quantity}.";
                        _logger.LogError(msg);
                        throw new InvalidOperationException(msg);
                    }

                    bf.Flower.Quantity -= required;
                    bf.Flower.UpdatedAt = DateTime.UtcNow;

                    _unitOfWork.Flowers.Update(bf.Flower);

                    _logger.LogInformation(
                        "Віднято {Count} квіток '{FlowerName}' (ID: {FlowerId})",
                        required, bf.Flower.Name, bf.Flower.Id);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            await _eventLogService.MarkEventAsProcessedAsync(eventId);

            await _cacheInvalidation.InvalidateAllAsync();

            _logger.LogInformation("Замовлення {OrderId} успішно оброблено", orderEvent.OrderId);
        }
    }
}