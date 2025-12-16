using CatalogService.BLL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Validators.FlowerValidation
{
    public class FlowerCreateUpdateValidator : AbstractValidator<FlowerCreateUpdateDto>
    {
        public FlowerCreateUpdateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Назва є обов'язковою.")
                .MaximumLength(100);

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Колір є обов'язковим.")
                .MaximumLength(50);

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Кількість не може бути від’ємною.");
        }
    }
}
