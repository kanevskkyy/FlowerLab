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
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Color is required.")
                .MaximumLength(50);

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");
        }
    }
}
