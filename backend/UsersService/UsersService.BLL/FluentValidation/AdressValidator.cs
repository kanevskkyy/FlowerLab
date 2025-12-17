using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.Models.Adresess;

namespace UsersService.BLL.FluentValidation
{
    public class AddressValidator : AbstractValidator<CreateAddressDto>
    {
        public AddressValidator()
        {
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адрсе є обов'язкова")
                .MaximumLength(200).WithMessage("Максимальна довжина адреси 200 символів");

            RuleFor(x => x.IsDefault)
                .NotNull().WithMessage("Необхідно вказати, чи адреса за замовчуванням");
        }
    }
}
