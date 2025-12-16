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
                .WithMessage("Мінімальна ціна повинна бути більшою або рівною 0");

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxPrice.HasValue)
                .WithMessage("Максимальна ціна повинна бути більшою або рівною 0");

            RuleForEach(x => x.Quantities)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Кількості повинні бути більші або рівні 0");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Номер сторінки повинен бути більшим за 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Розмір сторінки повинен бути більшим за 0");
        }
    }
}
