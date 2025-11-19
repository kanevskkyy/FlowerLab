using CatalogService.Domain.QueryParametrs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Validators
{
    using FluentValidation;

    public class BouquetQueryParametersValidator : AbstractValidator<BouquetQueryParameters>
    {
        public BouquetQueryParametersValidator()
        {
            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinPrice.HasValue)
                .WithMessage("MinPrice must be greater than or equal to 0");

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxPrice.HasValue)
                .WithMessage("MaxPrice must be greater than or equal to 0");

            RuleForEach(x => x.Quantities)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantities must be greater than or equal to 0");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("PageSize must be greater than 0");
        }
    }
}
