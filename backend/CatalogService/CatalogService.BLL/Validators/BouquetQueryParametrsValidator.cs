using CatalogService.Domain.QueryParametrs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Validators
{
    public class BouquetQueryParametersValidator : AbstractValidator<BouquetQueryParameters>
    {
        public BouquetQueryParametersValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);
            RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
            RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(x => x.MinPrice).When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);
            RuleFor(x => x.SortBy).Must(s => s == null || new[] { "price_asc", "price_desc", "newest" }.Contains(s))
                .WithMessage("SortBy must be 'price_asc', 'price_desc' or 'newest'");
        }
    }
}
