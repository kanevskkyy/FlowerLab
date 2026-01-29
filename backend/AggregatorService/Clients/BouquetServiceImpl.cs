using System;
using System.Linq;
using System.Threading.Tasks;
using AggregatorService.Clients.Interfaces;
using AggregatorService.DTOs;
using shared.localization;
using static BouquetGrpcService;
using static ReviewsByBouquetId;

namespace AggregatorService.Clients
{
    public class BouquetServiceImpl : IBouquetWithReviewsService
    {
        private BouquetGrpcService.BouquetGrpcServiceClient bouquetGrpcServiceClient;
        private ReviewsByBouquetId.ReviewsByBouquetIdClient reviewsByBouquetIdClient;
        private shared.localization.ILanguageProvider languageProvider;

        public BouquetServiceImpl(
            BouquetGrpcService.BouquetGrpcServiceClient bouquetGrpcServiceClient, 
            ReviewsByBouquetId.ReviewsByBouquetIdClient reviewsByBouquetIdClient,
            shared.localization.ILanguageProvider languageProvider)
        {
            this.bouquetGrpcServiceClient = bouquetGrpcServiceClient;
            this.reviewsByBouquetIdClient = reviewsByBouquetIdClient;
            this.languageProvider = languageProvider;
        }

        private Grpc.Core.Metadata GetMetadata()
        {
            var metadata = new Grpc.Core.Metadata();
            if (!string.IsNullOrEmpty(languageProvider.CurrentLanguage))
            {
                metadata.Add("accept-language", languageProvider.CurrentLanguage);
            }
            return metadata;
        }

        public async Task<BouquetInfoWithReviewsDTO> GetAggegatedBouquetInfo(Guid bouquetId)
        {
            var bouquetResponse = await bouquetGrpcServiceClient.GetBouquetByIdAsync(
                new GetBouquetRequest { Id = bouquetId.ToString() },
                GetMetadata());

            var reviewsResponse = await reviewsByBouquetIdClient.GetReviewsByBouquetIdAsync( 
                new ReviewBouquetIdGrpcRequest { Id = bouquetId.ToString() },
                GetMetadata());

            var aggregatedDto = new BouquetInfoWithReviewsDTO
            {
                Bouquet = bouquetResponse,
                Reviews = reviewsResponse
            };

            return aggregatedDto;
        }

        public async Task<GetBouquetsResponse> GetBouquetsAsync(GetBouquetsRequest request)
        {
            return await bouquetGrpcServiceClient.GetBouquetsAsync(request, GetMetadata());
        }
    }
}
