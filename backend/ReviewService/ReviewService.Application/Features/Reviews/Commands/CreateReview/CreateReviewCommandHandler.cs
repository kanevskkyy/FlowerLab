using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewService.Application.Interfaces.Commands;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;

namespace ReviewService.Application.Features.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler : ICommandHandler<CreateReviewCommand, Review>
    {
        private IReviewRepository reviewRepository;
        private readonly CheckIdInReviews.CheckIdInReviewsClient grpcClient;

        public CreateReviewCommandHandler(IReviewRepository reviewRepository, CheckIdInReviews.CheckIdInReviewsClient grpcClient)
        {
            this.reviewRepository = reviewRepository;
            this.grpcClient = grpcClient;
        }

        public async Task<Review> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var grpcResponse = await grpcClient.CheckIdAsync(new ReviewCheckIdRequest { Id = request.BouquetId.ToString() });
            if (!grpcResponse.IsValid) throw new InvalidOperationException($"Bouquet ID is invalid: {grpcResponse.ErrorMessage}");

            Review review = new Review(request.BouquetId, request.User, request.Rating, request.Comment);

            bool alreadyExists = await reviewRepository.HasUserReviewedBouquetAsync(review.User.UserId, review.BouquetId, cancellationToken);
            if (alreadyExists) throw new AlreadyExistsException("User has already reviewed this bouquet!");

            await reviewRepository.AddAsync(review, cancellationToken);
            return review;
        }
    }
}
