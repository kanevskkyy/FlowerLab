using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ReviewService.Application.Interfaces.Commands;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Helpers;
using ReviewService.Domain.Interfaces;

namespace ReviewService.Application.Features.Reviews.Commands.UpdateReview
{
    public class UpdateReviewWithIdCommandHandler : ICommandHandler<UpdateReviewWithIdCommand>
    {
        private IReviewRepository reviewRepository;

        public UpdateReviewWithIdCommandHandler(IReviewRepository reviewRepository)
        {
            this.reviewRepository = reviewRepository;
        }

        public async Task<Unit> Handle(UpdateReviewWithIdCommand request, CancellationToken cancellationToken)
        {
            Review? review = await reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException($"Відгук з ID {request.ReviewId} не знайдено!");

            review.UpdateComment(request.Comment, request.Rating);
            await reviewRepository.UpdateAsync(review, cancellationToken);

            return Unit.Value;
        }
    }
}
