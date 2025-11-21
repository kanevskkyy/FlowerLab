using CatalogService.BLL.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CatalogService.BLL.Validators
{
    public class BouquetCreateValidator : AbstractValidator<BouquetCreateDto>
    {
        private readonly string[] _allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };

        public BouquetCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.FlowerIds)
                .NotEmpty()
                .WithMessage("Букет повинен містити принаймні одну квітку.");

            RuleFor(x => x.FlowerQuantities)
                .NotEmpty()
                .WithMessage("Букет повинен містити принаймні одну кількість квітки.")
                .Must((dto, quantities) => quantities.Count == dto.FlowerIds.Count)
                .WithMessage("Кожна квітка повинна мати відповідну кількість.");

            RuleForEach(x => x.FlowerQuantities)
                .GreaterThan(0)
                .WithMessage("Кількість квітки повинна бути більше 0.");

            RuleFor(x => x.MainPhoto)
                .NotNull().WithMessage("Головне фото обов'язкове.")
                .Must(BeAValidImage).WithMessage("Головне фото має бути зображенням формату jpg, png, gif або webp.");

            RuleForEach(x => x.Images)
                .Must(BeAValidImage).WithMessage("Додаткові фото мають бути зображеннями формату jpg, png, gif або webp.");
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file == null) return false;
            return _allowedImageTypes.Contains(file.ContentType.ToLower());
        }
    }
}