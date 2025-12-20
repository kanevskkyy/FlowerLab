using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.BLL.DTO;
using FluentValidation;

namespace CatalogService.BLL.Validators.BouquetValidation
{
    public class BouquetSizeCreateValidator : AbstractValidator<BouquetSizeCreateDto>
    {
        public BouquetSizeCreateValidator()
        {
            RuleFor(x => x.SizeId)
                .NotEmpty()
                .WithMessage("Size ID is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.");

            RuleFor(x => x.FlowerIds)
                .NotEmpty()
                .WithMessage("Each size must contain at least one flower.");

            RuleFor(x => x.FlowerQuantities)
                .NotEmpty()
                .WithMessage("Each size must contain quantities for the flowers.")
                .Must((dto, quantities) => quantities != null && quantities.Count == dto.FlowerIds.Count)
                .WithMessage("Each flower must have a corresponding quantity.");

            RuleForEach(x => x.FlowerQuantities)
                .GreaterThan(0)
                .WithMessage("Flower quantity must be greater than 0.");

            RuleFor(x => x.FlowerIds)
                .Must(HaveUniqueFlowers)
                .WithMessage("Flowers in one size must be unique.");
        }

        private bool HaveUniqueFlowers(List<Guid> flowerIds)
        {
            if (flowerIds == null || !flowerIds.Any()) return true;
            return flowerIds.Distinct().Count() == flowerIds.Count;
        }
    }
}
