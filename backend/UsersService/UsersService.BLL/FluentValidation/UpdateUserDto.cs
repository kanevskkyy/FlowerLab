using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using UsersService.BLL.Models.Users;

namespace UsersService.BLL.FluentValidation
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024;

        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ім'я є обов'язковим")
                .MaximumLength(50).WithMessage("Ім'я не може перевищувати 50 символів");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Прізвище є обов'язковим")
                .MaximumLength(50).WithMessage("Прізвище не може перевищувати 50 символів");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+380\d{9}$")
                .WithMessage("Номер телефону має бути у форматі +380XXXXXXXXX")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Photo)
                .Must(BeAValidFile).WithMessage("Фото має бути валідним зображенням (.jpg, .jpeg, .png, .webp)")
                .Must(f => f.Length <= MaxFileSize).WithMessage("Розмір фото має бути менше 5 МБ")
                .When(x => x.Photo != null);
        }

        private bool BeAValidFile(IFormFile? file)
        {
            if (file == null) return true;

            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            return _permittedExtensions.Contains(extension);
        }
    }
}
