using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using OrderService.BLL.DTOs.GiftsDTOs;

namespace OrderService.BLL.FluentValidation
{
    public class GiftUpdateDtoValidator : AbstractValidator<GiftUpdateDto>
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public GiftUpdateDtoValidator()
        {
            RuleFor(g => g.Name)
                .NotEmpty().WithMessage("Назва подарунка є обов’язковою")
                .MaximumLength(100).WithMessage("Назва подарунка не може перевищувати 100 символів");

            RuleFor(g => g.AvailableCount)
                .GreaterThanOrEqualTo(0).WithMessage("Кількість подарунків не може бути від’ємною");

            When(g => g.Image != null, () =>
            {
                RuleFor(g => g.Image)
                    .Must(IsValidFileType).WithMessage("Недійсний формат зображення (дозволені: .jpg, .jpeg, .png, .webp)");
            });
        }

        private bool IsValidFileType(IFormFile? file)
        {
            if (file == null)
                return true;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(ext);
        }
    }
}
