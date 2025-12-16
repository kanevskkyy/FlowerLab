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
                .WithMessage("ID розміру обов'язковий.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Ціна повинна бути більше 0.");

            RuleFor(x => x.FlowerIds)
                .NotEmpty()
                .WithMessage("Кожен розмір повинен містити принаймні одну квітку.");

            RuleFor(x => x.FlowerQuantities)
                .NotEmpty()
                .WithMessage("Кожен розмір повинен містити кількість для квіток.")
                .Must((dto, quantities) => quantities != null && quantities.Count == dto.FlowerIds.Count)
                .WithMessage("Кожна квітка повинна мати відповідну кількість.");

            RuleForEach(x => x.FlowerQuantities)
                .GreaterThan(0)
                .WithMessage("Кількість квітки повинна бути більше 0.");

            RuleFor(x => x.FlowerIds)
                .Must(HaveUniqueFlowers)
                .WithMessage("Квіти в одному розмірі не повинні повторюватися.");
        }

        private bool HaveUniqueFlowers(List<Guid> flowerIds)
        {
            if (flowerIds == null || !flowerIds.Any()) return true;
            return flowerIds.Distinct().Count() == flowerIds.Count;
        }
    }
}
