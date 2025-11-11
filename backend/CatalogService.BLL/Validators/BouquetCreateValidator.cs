using CatalogService.BLL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Validators
{
    public class BouquetCreateValidator : AbstractValidator<BouquetCreateDto>
    {
        public BouquetCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.FlowerIds)
                .NotEmpty()
                .WithMessage("Bouquet must contain at least one flower.");

            RuleFor(x => x.FlowerQuantities)
                .NotEmpty()
                .WithMessage("Bouquet must contain at least one flower quantity.")
                .Must((dto, quantities) => quantities.Count == dto.FlowerIds.Count)
                .WithMessage("Each flower must have a corresponding quantity.");

            RuleForEach(x => x.FlowerQuantities)
                .GreaterThan(0)
                .WithMessage("Flower quantity must be greater than 0.");
        }
    }
}
