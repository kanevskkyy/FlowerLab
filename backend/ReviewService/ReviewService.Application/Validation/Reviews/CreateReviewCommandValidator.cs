using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ReviewService.Application.Features.Reviews.Commands.CreateReview;

namespace ReviewService.Application.Validation.Reviews
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.BouquetId)
                .NotEmpty().WithMessage("Bouquet identifier (BouquetId) is required.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment cannot be empty.")
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
        }
    }
}
