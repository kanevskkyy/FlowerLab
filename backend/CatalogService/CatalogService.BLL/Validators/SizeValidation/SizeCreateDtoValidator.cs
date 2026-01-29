using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.BLL.DTO;
using FluentValidation;

namespace CatalogService.BLL.Validators.SizeValidation
{
    public class SizeCreateDtoValidator : AbstractValidator<SizeCreateDto>
    {
        public SizeCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required");
        }

    }

}
