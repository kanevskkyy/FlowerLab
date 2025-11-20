using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.BLL.DTO;
using FluentValidation;

namespace CatalogService.BLL.Validators
{
    public class SizeCreateDtoValidator : AbstractValidator<SizeCreateDto>
    {
        public SizeCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Назва є обов'язковою")
                .MaximumLength(50).WithMessage("Назва повинна містити максимум 50 символів");
        }

    }

}
