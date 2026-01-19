using CatalogService.BLL.DTO.Bouquet;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Validators.BouquetValidation
{
    public class BouquetSizeUpdateValidator : AbstractValidator<BouquetSizeUpdateDto>
    {
        private readonly string[] _allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };

        public BouquetSizeUpdateValidator()
        {
            RuleFor(x => x.SizeId)
                .NotEmpty()
                .WithMessage("Size ID is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.");

            RuleFor(x => x.FlowerIds)
                .NotEmpty()
                .WithMessage("At least one flower must be specified.");

            RuleFor(x => x.FlowerQuantities)
                .NotEmpty()
                .WithMessage("Flower quantities must be specified.");

            RuleFor(x => x)
                .Must(x => x.FlowerIds.Count == x.FlowerQuantities.Count)
                .WithMessage("The number of flower IDs must match the number of quantities.");

            RuleForEach(x => x.FlowerQuantities)
                .GreaterThan(0)
                .WithMessage("Flower quantity must be greater than 0.");

            When(x => x.MainImage != null, () =>
            {
                RuleFor(x => x.MainImage)
                    .Must(BeAValidImage)
                    .WithMessage("Main image must be of type jpg, png, gif, or webp.");
            });

     
            RuleForEach(x => x.AdditionalImages)
                .Must(BeAValidImage)
                .WithMessage("New images must be of type jpg, png, gif, or webp.");

            RuleFor(x => x.AdditionalImages)
                .Must(images => images == null || images.Count <= 3)
                .WithMessage("You can upload a maximum of 3 new images per size.");
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file == null) return false;
            return _allowedImageTypes.Contains(file.ContentType.ToLower());
        }
    }
}
