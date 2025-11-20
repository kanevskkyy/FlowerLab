using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ReviewService.Domain.ValueObjects;

namespace ReviewService.Application.Validation.Additional
{
    public class UserInfoValidator : AbstractValidator<UserInfo>
    {
        public UserInfoValidator()
        {
            RuleFor(u => u.UserId)
                .NotEmpty().WithMessage("Ідентифікатор користувача (UserId) обов'язковий.");

            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("Ім'я обов'язкове.")
                .MaximumLength(100).WithMessage("Ім'я не може перевищувати 100 символів.");

            RuleFor(u => u.LastName)
                .NotEmpty().WithMessage("Прізвище обов'язкове.")
                .MaximumLength(100).WithMessage("Прізвище не може перевищувати 100 символів.");

            RuleFor(x => x.PhotoUrl)
                .NotEmpty().WithMessage("URL фотографії обов'язковий.")
                .Must(BeValidUrl).WithMessage("URL фотографії має бути дійсним URL.")
                .Must(x => x.Contains("res.cloudinary")).WithMessage("URL фотографії має бути посиланням Cloudinary.");
        }

        private bool BeValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var tmp)
                   && (tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps);
        }
    }
}
