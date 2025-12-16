using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.Models.Auth;

namespace UsersService.BLL.FluentValidation
{
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId є обов'язковим");
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token є обов'язковим");
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Новий пароль є обов'язковим")
                .MinimumLength(8).WithMessage("Пароль має бути не менше 8 символів");
        }
    }
}
