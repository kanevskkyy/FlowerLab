using AggregatorService.Clients.Interfaces;
using AggregatorService.DTOs;
using static BouquetGrpcService;
using static ReviewsByBouquetId;

namespace AggregatorService.Clients
{
    public class BouquetServiceImpl : IBouquetWithReviewsService
    {
        private BouquetGrpcService.BouquetGrpcServiceClient bouquetGrpcServiceClient;
        private ReviewsByBouquetId.ReviewsByBouquetIdClient reviewsByBouquetIdClient;

        public BouquetServiceImpl(BouquetGrpcService.BouquetGrpcServiceClient bouquetGrpcServiceClient, ReviewsByBouquetId.ReviewsByBouquetIdClient reviewsByBouquetIdClient)
        {
            this.bouquetGrpcServiceClient = bouquetGrpcServiceClient;
            this.reviewsByBouquetIdClient = reviewsByBouquetIdClient;
        }

        public async Task<BouquetInfoWithReviewsDTO> GetAggegatedBouquetInfo(Guid bouquetId)
        {
            var bouquetResponse = await bouquetGrpcServiceClient.GetBouquetByIdAsync(new GetBouquetRequest { Id = bouquetId.ToString() });

            var reviewsResponse = await reviewsByBouquetIdClient.GetReviewsByBouquetIdAsync( new ReviewBouquetIdGrpcRequest { Id = bouquetId.ToString() });

            var aggregatedDto = new BouquetInfoWithReviewsDTO
            {
                Bouquet = bouquetResponse,
                Reviews = reviewsResponse
            };

            return aggregatedDto;
        }
    }
}
