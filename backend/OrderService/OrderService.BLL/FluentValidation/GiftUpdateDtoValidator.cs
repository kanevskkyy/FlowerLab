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
                .NotEmpty().WithMessage("Gift name is required");

            RuleFor(g => g.AvailableCount)
                .GreaterThanOrEqualTo(0).WithMessage("Gift count cannot be negative");

            When(g => g.Image != null, () =>
            {
                RuleFor(g => g.Image)
                    .Must(IsValidFileType).WithMessage("Invalid image format (allowed: .jpg, .jpeg, .png, .webp)");
            });

            RuleFor(g => g.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");
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
