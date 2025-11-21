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
            if (!grpcResponse.IsValid)
                throw new InvalidOperationException($"ID букета недійсний: {grpcResponse.ErrorMessage}");

            if (request.User == null)
            {
                throw new UnauthorizedAccessException("User information is missing from the request context.");
            }

            Review review = new Review(request.BouquetId, request.User, request.Rating, request.Comment);

            bool alreadyExists = await reviewRepository.HasUserReviewedBouquetAsync(review.User.UserId, review.BouquetId, cancellationToken);
            if (alreadyExists)
                throw new AlreadyExistsException("Користувач вже залишив відгук для цього букету!");
            if (alreadyExists) throw new InvalidOperationException("User has already reviewed this bouquet!"); 

            await reviewRepository.AddAsync(review, cancellationToken);
            return review;
        }
    }
}
