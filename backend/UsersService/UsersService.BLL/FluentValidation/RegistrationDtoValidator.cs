using FluentValidation;
using UsersService.BLL.Models;

namespace UsersService.BLL.FluentValidation 
{
    public class RegistrationDtoValidator : AbstractValidator<RegistrationDto>
    {
        public RegistrationDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ім'я є обов'язковим.")
                .MaximumLength(50).WithMessage("Ім'я не може перевищувати 50 символів.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Прізвище є обов'язковим.")
                .MaximumLength(50).WithMessage("Прізвище не може перевищувати 50 символів.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email є обов'язковим.")
                .EmailAddress().WithMessage("Невірний формат email.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль є обов'язковим.")
                .MinimumLength(8).WithMessage("Пароль має містити мінімум 8 символів."); // 👍

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Номер телефону є обов'язковим.")
                .Matches(@"^\+?(\d[\s-]?){8,15}$")
                .WithMessage("Невірний формат номера телефону.");
        }
    }


}