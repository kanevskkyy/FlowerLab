using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using UsersService.BLL.Models;

namespace UsersService.BLL.FluentValidation
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Старий пароль є обов'язковим.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Новий пароль є обов'язковим.")
                .MinimumLength(8).WithMessage("Пароль має містити мінімум 8 символів.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword).WithMessage("Паролі не співпадають.");
        }
    }
}
