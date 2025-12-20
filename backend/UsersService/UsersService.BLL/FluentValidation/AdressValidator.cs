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
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200).WithMessage("Maximum address length is 200 characters");

            RuleFor(x => x.IsDefault)
                .NotNull().WithMessage("You must specify whether the address is default");
        }
    }
}
