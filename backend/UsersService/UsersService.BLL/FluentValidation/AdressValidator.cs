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
            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Країна є обов'язковою")
                .MaximumLength(100).WithMessage("Країна не може бути довшою за 100 символів");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Місто є обов'язковим")
                .MaximumLength(100);

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Вулиця є обов'язковою")
                .MaximumLength(200);

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Поштовий індекс є обов'язковим");

            RuleFor(x => x.IsDefault)
                .NotNull().WithMessage("Необхідно вказати, чи адреса за замовчуванням");
        }
    }
}
