using System;
using System.Collections.Generic;
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
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<ReviewsByBouquetServiceImpl> _logger;

        public ReviewsByBouquetServiceImpl(
            IReviewRepository reviewRepository,
            ILogger<ReviewsByBouquetServiceImpl> logger)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

        public override async Task<ReviewsListGrpcResponse> GetReviewsByBouquetId(
            ReviewBouquetIdGrpcRequest request,
            ServerCallContext context)
        {
            _logger.LogInformation($"Received gRPC request for BouquetId: {request.Id}");

            Guid bouquetId;
            try
            {
                bouquetId = Guid.Parse(request.Id);
                _logger.LogInformation($"Parsed BouquetId as Guid: {bouquetId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to parse BouquetId: {ex.Message}");
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid BouquetId format"));
            }

            var queryParams = new ReviewQueryParameters
            {
                BouquetId = bouquetId,
                Status = ReviewStatus.Confirmed
            };

            _logger.LogInformation($"Calling repository with BouquetId: {queryParams.BouquetId}");

            var reviews = await _reviewRepository.GetReviewsAsync(queryParams);

            _logger.LogInformation($"Repository returned {reviews.Items.Count} reviews, Total: {reviews.TotalCount}");

            var grpcResponse = new ReviewsListGrpcResponse();

            foreach (var review in reviews.Items)
            {
                _logger.LogInformation($"Processing review ID: {review.Id}, BouquetId: {review.BouquetId}, Status: {review.Status}");

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

            _logger.LogInformation($"Returning {grpcResponse.Reviews.Count} reviews in gRPC response");

            return grpcResponse;
        }
    }
}