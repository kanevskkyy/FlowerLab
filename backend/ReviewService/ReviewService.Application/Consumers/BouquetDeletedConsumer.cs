using MassTransit;
using FlowerLab.Shared.Events;
using ReviewService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using shared.events.EventService;

namespace ReviewService.Application.Consumers
{
    public class BouquetDeletedConsumer : IConsumer<BouquetDeletedEvent>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IEventLogService _eventLogService;
        private readonly ILogger<BouquetDeletedConsumer> _logger;

        public BouquetDeletedConsumer(
            IReviewRepository reviewRepository, IEventLogService eventLogService, ILogger<BouquetDeletedConsumer> logger)
        {
            _reviewRepository = reviewRepository;
            _eventLogService = eventLogService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BouquetDeletedEvent> context)
        {
            var eventId = context.Message.EventId;

            if (await _eventLogService.HasEventProcessedAsync(eventId, context.CancellationToken))
            {
                _logger.LogWarning("[REVIEWS] Event {EventId} has already been processed. Skipping…", eventId);
                return;
            }

            var bouquetId = context.Message.BouquetId;
            _logger.LogInformation($"[REVIEWS] Received BouquetDeletedEvent for bouquet: {bouquetId}");

            try
            {
                await _reviewRepository.DeleteByBouquetIdAsync(bouquetId, context.CancellationToken);
                await _eventLogService.MarkEventAsProcessedAsync(eventId, context.CancellationToken);

                _logger.LogInformation($"[REVIEWS] Deleted reviews for bouquet: {bouquetId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[REVIEWS] Error deleting reviews");
                throw;
            }
        }
    }
}
