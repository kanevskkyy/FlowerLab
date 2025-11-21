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
    public class GiftCreateDtoValidator : AbstractValidator<GiftCreateDto>
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public GiftCreateDtoValidator()
        {
            RuleFor(g => g.Name)
                .NotEmpty().WithMessage("Назва подарунка є обов’язковою")
                .MaximumLength(100).WithMessage("Назва подарунка не може перевищувати 100 символів");

            RuleFor(g => g.AvailableCount)
                .GreaterThanOrEqualTo(0).WithMessage("Кількість подарунків не може бути від’ємною");

            RuleFor(g => g.Image)
                .NotNull().WithMessage("Файл зображення подарунка є обов’язковим")
                .Must(IsValidFileType).WithMessage("Недійсний формат зображення (дозволені: .jpg, .jpeg, .png, .webp)");

            RuleFor(g => g.Price)
              .GreaterThan(0).WithMessage("Ціна має бути більшою за 0");
        }

        private bool IsValidFileType(IFormFile? file)
        {
            if (file == null)
                return false;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(ext);
        }
    }
}
