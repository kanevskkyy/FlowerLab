using CatalogService.BLL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Validators
{
    public class BouquetUpdateValidator : AbstractValidator<BouquetUpdateDto>
    {
        public BouquetUpdateValidator()
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
        }
    }
}
