using MassTransit;
using FlowerLab.Shared.Events;
using ReviewService.Domain.Interfaces;

namespace ReviewService.Application.Consumers
{
    public class BouquetDeletedConsumer : IConsumer<BouquetDeletedEvent>
    {
        private readonly IReviewRepository _reviewRepository;

        public BouquetDeletedConsumer(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task Consume(ConsumeContext<BouquetDeletedEvent> context)
        {
            var bouquetId = context.Message.BouquetId;
            
            // Видаляємо всі відгуки для цього букета
            await _reviewRepository.DeleteByBouquetIdAsync(bouquetId, context.CancellationToken);
            
            // Можна додати логування
            Console.WriteLine($"Deleted reviews for bouquet: {bouquetId}");
        }
    }
}