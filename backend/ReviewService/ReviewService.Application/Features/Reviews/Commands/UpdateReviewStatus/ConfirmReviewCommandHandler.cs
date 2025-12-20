using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewService.Application.Interfaces.Commands;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;

namespace ReviewService.Application.Features.Reviews.Commands.UpdateReviewStatus
{
    public class ConfirmReviewCommandHandler : ICommandHandler<ConfirmReviewCommand, Review>
    {
        private IReviewRepository reviewRepository;

        public ConfirmReviewCommandHandler(IReviewRepository reviewRepository)
        {
            this.reviewRepository = reviewRepository;
        }

        public async Task<Review> Handle(ConfirmReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Review with ID {request.ReviewId} was not found.");

            review.Confirm();

            await reviewRepository.UpdateAsync(review, cancellationToken);
            return review;
        }
    }
}
