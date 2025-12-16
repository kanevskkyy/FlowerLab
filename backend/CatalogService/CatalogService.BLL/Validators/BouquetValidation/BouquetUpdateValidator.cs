using CatalogService.BLL.DTO;
using CatalogService.BLL.Validators.BouquetValidation;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Validators
{
    public class BouquetUpdateValidator : AbstractValidator<BouquetUpdateDto>
    {
        private readonly string[] _allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };

        public BouquetUpdateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Назва букета обов'язкова.")
                .MaximumLength(100)
                .WithMessage("Назва букета не може перевищувати 100 символів.");

            RuleFor(x => x.Sizes)
                .NotEmpty()
                .WithMessage("Букет повинен мати принаймні один розмір.");

            RuleForEach(x => x.Sizes)
                .SetValidator(new BouquetSizeCreateValidator());

            RuleFor(x => x.Sizes)
                .Must(HaveUniqueSize)
                .WithMessage("Розміри не повинні повторюватися.");

            When(x => x.MainPhoto != null, () =>
            {
                RuleFor(x => x.MainPhoto)
                    .Must(BeAValidImage)
                    .WithMessage("Головне фото має бути зображенням формату jpg, png, gif або webp.");
            });

            RuleForEach(x => x.NewImages)
                .Must(BeAValidImage)
                .WithMessage("Додаткові фото мають бути зображеннями формату jpg, png, gif або webp.");

            RuleFor(x => x.NewImages)
                .Must(images => images == null || images.Count <= 3)
                .WithMessage("Можна завантажити максимум 3 додаткових зображення.");
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file == null) return false;
            return _allowedImageTypes.Contains(file.ContentType.ToLower());
        }

        private bool HaveUniqueSize(List<BouquetSizeCreateDto> sizes)
        {
            if (sizes == null || !sizes.Any()) return true;
            return sizes.Select(s => s.SizeId).Distinct().Count() == sizes.Count;
        }
    }
}
