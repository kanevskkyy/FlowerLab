using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReviewService.Application.Interfaces.Commands;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers; 
using ReviewService.Domain.Interfaces;

namespace ReviewService.Application.Features.Reviews.Commands.DeleteReview
{
    public class DeleteReviewCommandHandler : ICommandHandler<DeleteReviewCommand>
    {
        private IReviewRepository reviewRepository;

        public DeleteReviewCommandHandler(IReviewRepository reviewRepository)
        {
            this.reviewRepository = reviewRepository;
        }

        public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            Review? review = await reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Review with ID {request.ReviewId} was not found!");

            bool isAdmin = request.RequesterRole == "Admin";
            bool isOwner = review.User.UserId == request.RequesterId;

            if (!isOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this review.");
            }

            await reviewRepository.DeleteAsync(request.ReviewId, cancellationToken);
            return Unit.Value;
        }
    }
}