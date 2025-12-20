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
                .WithMessage("Bouquet name is required.")
                .MaximumLength(100)
                .WithMessage("Bouquet name cannot exceed 100 characters.");

            RuleFor(x => x.Sizes)
                .NotEmpty()
                .WithMessage("The bouquet must have at least one size.");

            RuleForEach(x => x.Sizes)
                .SetValidator(new BouquetSizeCreateValidator());

            RuleFor(x => x.Sizes)
                .Must(HaveUniqueSize)
                .WithMessage("Sizes must be unique.");

            When(x => x.MainPhoto != null, () =>
            {
                RuleFor(x => x.MainPhoto)
                    .Must(BeAValidImage)
                    .WithMessage("Main photo must be an image of type jpg, png, gif, or webp.");
            });

            RuleForEach(x => x.NewImages)
                .Must(BeAValidImage)
                .WithMessage("Additional photos must be images of type jpg, png, gif, or webp.");

            RuleFor(x => x.NewImages)
                .Must(images => images == null || images.Count <= 3)
                .WithMessage("You can upload a maximum of 3 additional images.");
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
