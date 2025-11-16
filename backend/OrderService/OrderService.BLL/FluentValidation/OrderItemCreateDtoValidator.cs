using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using OrderService.BLL.DTOs.OrderDTOs;

namespace OrderService.BLL.FluentValidation
{
    public class OrderItemCreateDtoValidator : AbstractValidator<OrderItemCreateDto>
    {
        public OrderItemCreateDtoValidator()
        {
            RuleFor(i => i.BouquetId)
                .NotEmpty().WithMessage("BouquetId is required");

            RuleFor(i => i.BouquetName)
                .NotEmpty().WithMessage("Bouquet name is required")
                .MaximumLength(100).WithMessage("Bouquet name cannot exceed 100 characters");

            RuleFor(i => i.BouquetImage)
                .NotEmpty().WithMessage("Bouquet image URL is required")
                .Must(BeAValidUrl).WithMessage("Invalid image URL format")
                .Must(BeCloudinaryUrl).WithMessage("Image URL must be hosted on Cloudinary");

            RuleFor(i => i.Price)
                .GreaterThan(0).WithMessage("The price must be greater than 0");

            RuleFor(i => i.Count)
                .GreaterThan(0).WithMessage("The quantity must be greater than 0");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        private bool BeCloudinaryUrl(string url)
        {
            return url.Contains("res.cloudinary.com");
        }
    }
}
