using CatalogService.Domain.QueryParametrs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Validators.BouquetValidation
{
    using FluentValidation;

    public class BouquetQueryParametersValidator : AbstractValidator<BouquetQueryParameters>
    {
        public BouquetQueryParametersValidator()
        {
            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinPrice.HasValue)
                .WithMessage("Minimum price must be greater than or equal to 0");

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxPrice.HasValue)
                .WithMessage("Maximum price must be greater than or equal to 0");

            RuleForEach(x => x.Quantities)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantities must be greater than or equal to 0");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0");
        }
    }
}
