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
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Flowers).NotEmpty().WithMessage("Bouquet must contain at least one flower.");
            RuleForEach(x => x.Flowers).SetValidator(new FlowerQuantityValidator());
        }
    }
}
