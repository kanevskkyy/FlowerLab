using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ReviewService.Application.Features.Reviews.Commands.CreateReview;
using ReviewService.Application.Validation.Additional;

namespace ReviewService.Application.Validation.Reviews
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.BouquetId)
                .NotEmpty().WithMessage("Ідентифікатор букета (BouquetId) обов'язковий.");

            RuleFor(x => x.User)
                .NotNull().WithMessage("Інформація про користувача обов'язкова.")
                .SetValidator(new UserInfoValidator());

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Рейтинг має бути між 1 та 5.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Коментар не може бути порожнім.")
                .MaximumLength(500).WithMessage("Коментар не може перевищувати 500 символів.");
        }
    }
}
