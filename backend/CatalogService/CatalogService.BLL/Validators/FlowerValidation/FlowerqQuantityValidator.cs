using CatalogService.BLL.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Validators.FlowerValidation
{
    public class FlowerQuantityValidator : AbstractValidator<FlowerQuantityDto>
    {
        public FlowerQuantityValidator()
        {
            RuleFor(x => x.FlowerId)
                .NotEmpty()
                .WithMessage("Flower ID is required.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity must be greater than 0.");
        }
    }
}
