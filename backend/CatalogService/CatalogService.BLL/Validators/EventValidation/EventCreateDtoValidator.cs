using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.BLL.DTO;
using FluentValidation;

namespace CatalogService.BLL.Validators.EventValidation
{
    public class EventCreateDtoValidator : AbstractValidator<EventCreateDto>
    {
        public EventCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must be at most 100 characters long");
        }
    }
}
