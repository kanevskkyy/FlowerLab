using CatalogService.DAL.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Logging;
using shared.events;

namespace CatalogService.BLL.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(IUnitOfWork unitOfWork, ILogger<OrderCreatedConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
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
                    if (bf.Flower.Quantity < bf.Quantity * item.Count)
                    {
                        var msg = $"Недостатньо квітів '{bf.Flower.Name}'. Запитано {bf.Quantity * item.Count}, доступно {bf.Flower.Quantity}.";
                        _logger.LogError(msg);
                        throw new InvalidOperationException(msg);
                    }

                    bf.Flower.Quantity -= bf.Quantity * item.Count;
                    bf.Flower.UpdatedAt = DateTime.UtcNow.ToUniversalTime();

                    _unitOfWork.Flowers.Update(bf.Flower);
                    _logger.LogInformation("Віднято {Count} квіток '{FlowerName}' (ID: {FlowerId})",
                        bf.Quantity * item.Count, bf.Flower.Name, bf.Flower.Id);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Замовлення {OrderId} успішно оброблено", orderEvent.OrderId);
        }
    }
}