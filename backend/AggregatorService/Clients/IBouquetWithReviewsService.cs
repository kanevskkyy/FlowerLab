using AggregatorService.DTOs;

namespace AggregatorService.Clients
{
    public interface IBouquetWithReviewsService
    {
        Task<BouquetInfoWithReviewsDTO> GetAggegatedBouquetInfo(Guid bouquetId);
    }
}
