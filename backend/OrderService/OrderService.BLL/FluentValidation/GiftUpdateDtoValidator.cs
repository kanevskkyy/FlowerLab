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
                .NotEmpty().WithMessage("Gift name is required")
                .MaximumLength(100).WithMessage("Gift name cannot exceed 100 characters");

            When(g => g.Image != null, () =>
            {
                RuleFor(g => g.Image)
                    .Must(IsValidFileType).WithMessage("Invalid image format (only .jpg, .jpeg, .png, .webp are allowed)");
            });
        }

        private bool IsValidFileType(IFormFile? file)
        {
            if (file == null) return true;
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(ext);
        }
    }
}
