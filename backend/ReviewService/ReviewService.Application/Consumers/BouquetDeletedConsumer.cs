using MassTransit;
using FlowerLab.Shared.Events;
using ReviewService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ReviewService.Application.Consumers
{
    public class BouquetDeletedConsumer : IConsumer<BouquetDeletedEvent>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<BouquetDeletedConsumer> _logger;

        public BouquetDeletedConsumer(
            IReviewRepository reviewRepository,
            ILogger<BouquetDeletedConsumer> logger)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BouquetDeletedEvent> context)
        {
            var bouquetId = context.Message.BouquetId;

            _logger.LogInformation($"[REVIEWS] ✓ Отримано BouquetDeletedEvent для букету: {bouquetId}");
            Console.WriteLine($"[REVIEWS] ✓ Отримано BouquetDeletedEvent для букету: {bouquetId}");

            try
            {
                await _reviewRepository.DeleteByBouquetIdAsync(bouquetId, context.CancellationToken);
                _logger.LogInformation($"[REVIEWS] ✓ Видалено відгуки для букету: {bouquetId}");
                Console.WriteLine($"[REVIEWS] ✓ Видалено відгуки для букету: {bouquetId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[REVIEWS] ✗ Помилка видалення відгуків: {ex.Message}");
                Console.WriteLine($"[REVIEWS] ✗ Помилка видалення відгуків: {ex.Message}");
                throw;
            }
        }
    }
}