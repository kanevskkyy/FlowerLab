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
                .NotEmpty().WithMessage("Gift name is required")
                .MaximumLength(100).WithMessage("Gift name cannot exceed 100 characters");

            RuleFor(g => g.AvailableCount)
                .GreaterThanOrEqualTo(0).WithMessage("Gift count cannot be negative");

            RuleFor(g => g.Image)
                .NotNull().WithMessage("Gift image file is required")
                .Must(IsValidFileType).WithMessage("Invalid image format (allowed: .jpg, .jpeg, .png, .webp)");

            RuleFor(g => g.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");
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
