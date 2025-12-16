using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.Models.Email;

namespace UsersService.BLL.FluentValidation
{
    public class ResendConfirmEmailDtoValidator : AbstractValidator<ResendConfirmEmailDto>
    {
        public ResendConfirmEmailDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email є обов'язковим")
                .EmailAddress().WithMessage("Некоректний формат email");
        }
    }
}
