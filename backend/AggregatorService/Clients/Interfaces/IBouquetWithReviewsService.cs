using AggregatorService.DTOs;

namespace AggregatorService.Clients.Interfaces
{
    public interface IBouquetWithReviewsService
    {
        Task<BouquetInfoWithReviewsDTO> GetAggegatedBouquetInfo(Guid bouquetId);
    }
}
