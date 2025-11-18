using FluentValidation;
using UsersService.BLL.Models;

namespace UsersService.BLL.FluentValidation 
{
    // Успадковуємо від AbstractValidator
    public class RegistrationDtoValidator : AbstractValidator<RegistrationDto>
    {
        public RegistrationDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            
            // Основна валідація email
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            
            // Пароль перевіряється ASP.NET Identity, тут лише перевірка на порожнє значення
            RuleFor(x => x.Password).NotEmpty(); 
            
            // Валідація телефону: обов'язковий та відповідає простому формату
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Номер телефону є обов'язковим.")
                .Matches(@"^\+?(\d[\s-]?){8,15}$") 
                .WithMessage("Невірний формат номера телефону.");
        }
    }
}