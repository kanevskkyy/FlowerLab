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

        public CreateReviewCommandHandler(IReviewRepository reviewRepository)
        {
            this.reviewRepository = reviewRepository;
        }

        public async Task<Review> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            Review review = new Review(request.BouquetId, request.User, request.Rating, request.Comment);

            bool alreadyExists = await reviewRepository.HasUserReviewedBouquetAsync(review.User.UserId, review.BouquetId, cancellationToken);
            if (alreadyExists) throw new AlreadyExistsException("User has already reviewed this bouquet!");

            await reviewRepository.AddAsync(review, cancellationToken);
            return review;
        }
    }
}
