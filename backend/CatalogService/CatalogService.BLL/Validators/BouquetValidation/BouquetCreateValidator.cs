using CatalogService.BLL.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CatalogService.BLL.Validators.BouquetValidation
{
    public class BouquetCreateValidator : AbstractValidator<BouquetCreateDto>
    {
        private readonly string[] _allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };

        public BouquetCreateValidator()
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

            RuleFor(x => x.MainPhoto)
                .NotNull()
                .WithMessage("Main photo is required.")
                .Must(BeAValidImage)
                .WithMessage("Main photo must be an image of type jpg, png, gif, or webp.");

            RuleForEach(x => x.Images)
                .Must(BeAValidImage)
                .WithMessage("Additional photos must be images of type jpg, png, gif, or webp.");

            RuleFor(x => x.Images)
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
