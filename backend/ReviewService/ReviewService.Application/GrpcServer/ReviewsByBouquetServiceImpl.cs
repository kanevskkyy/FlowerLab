using System;
using System.Threading.Tasks;
using Grpc.Core;
using ReviewService.Domain.Entities.QueryParameters;
using ReviewService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using ReviewService.Domain.Entities;

namespace ReviewService.Application.GrpcServer
{
    public class ReviewsByBouquetServiceImpl : ReviewsByBouquetId.ReviewsByBouquetIdBase
    {
        private IReviewRepository reviewRepository;
        private ILogger<ReviewsByBouquetServiceImpl> logger;

        public ReviewsByBouquetServiceImpl(
            IReviewRepository reviewRepository,
            ILogger<ReviewsByBouquetServiceImpl> logger)
        {
            this.reviewRepository = reviewRepository;
            this.logger = logger;
        }

        public override async Task<ReviewsListGrpcResponse> GetReviewsByBouquetId(
            ReviewBouquetIdGrpcRequest request,
            ServerCallContext context)
        {
            logger.LogInformation("Received gRPC request for BouquetId: {BouquetId}", request.Id);

            Guid bouquetId;
            try
            {
                bouquetId = Guid.Parse(request.Id);
                logger.LogInformation("Parsed BouquetId to Guid: {BouquetId}", bouquetId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to parse BouquetId: {BouquetId}", request.Id);
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Невірний формат BouquetId"));
            }

            var queryParams = new ReviewQueryParameters
            {
                BouquetId = bouquetId,
                Status = ReviewStatus.Confirmed
            };

            logger.LogInformation("Calling repository with BouquetId: {BouquetId}", queryParams.BouquetId);

            var reviews = await reviewRepository.GetReviewsAsync(queryParams);

            logger.LogInformation("Repository returned {Count} reviews, Total: {TotalCount}", reviews.Items.Count, reviews.TotalCount);

            var grpcResponse = new ReviewsListGrpcResponse();

            foreach (var review in reviews.Items)
            {
                logger.LogInformation("Processing review ID: {ReviewId}, BouquetId: {BouquetId}, Status: {Status}",
                    review.Id, review.BouquetId, review.Status);

                var reviewGrpcResponse = new ReviewGrpcResponse
                {
                    Id = review.Id.ToString(),
                    Comment = review.Comment ?? "",
                    Rating = review.Rating,
                    CreatedAt = review.CreatedAt.ToString("O"),
                    User = new UserGrpcResponse
                    {
                        Id = review.User.UserId.ToString(),
                        FirstName = review.User.FirstName ?? "",
                        LastName = review.User.LastName ?? "",
                        PhotoUrl = review.User.PhotoUrl ?? ""
                    }
                };

                grpcResponse.Reviews.Add(reviewGrpcResponse);
            }

            logger.LogInformation("Returning {Count} reviews in gRPC response", grpcResponse.Reviews.Count);

            return grpcResponse;
        }
    }
}