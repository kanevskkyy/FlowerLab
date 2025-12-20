using System;
using System.Threading;
using System.Threading.Tasks;
using ReviewService.Application.GrpcClient;
using ReviewService.Application.Interfaces.Commands;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;

namespace ReviewService.Application.Features.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler : ICommandHandler<CreateReviewCommand, Review>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly CheckIdInReviews.CheckIdInReviewsClient _checkIdGrpcClient;
        private readonly CheckOrderServiceGRPC.CheckOrderServiceGRPCClient _checkOrderGrpcClient;

        public CreateReviewCommandHandler(
            IReviewRepository reviewRepository,
            CheckIdInReviews.CheckIdInReviewsClient checkIdGrpcClient,
            CheckOrderServiceGRPC.CheckOrderServiceGRPCClient checkOrderGrpcClient)
        {
            _reviewRepository = reviewRepository;
            _checkIdGrpcClient = checkIdGrpcClient;
            _checkOrderGrpcClient = checkOrderGrpcClient;
        }

        public async Task<Review> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            if (request.User == null)
                throw new UnauthorizedAccessException("User information is missing from the request context.");

            var checkIdResponse = await _checkIdGrpcClient.CheckIdAsync(
                new ReviewCheckIdRequest { Id = request.BouquetId.ToString() });

            if (!checkIdResponse.IsValid)
                throw new InvalidOperationException($"Bouquet ID is invalid: {checkIdResponse.ErrorMessage}");

            var orderCheckResponse = await _checkOrderGrpcClient.HasUserOrderedBouquetAsync(
                new UserOrderCheckRequestMessage
                {
                    UserId = request.User.UserId.ToString(),
                    BouquetId = request.BouquetId.ToString()
                });

            if (!orderCheckResponse.HasOrdered)
                throw new InvalidOperationException("User has not ordered this bouquet; review is forbidden.");

            bool alreadyReviewed = await _reviewRepository.HasUserReviewedBouquetAsync(
                request.User.UserId, request.BouquetId, cancellationToken);

            if (alreadyReviewed)
                throw new AlreadyExistsException("User has already left a review for this bouquet!");

            var review = new Review(request.BouquetId, request.User, request.Rating, request.Comment);

            await _reviewRepository.AddAsync(review, cancellationToken);

            return review;
        }
    }
}
