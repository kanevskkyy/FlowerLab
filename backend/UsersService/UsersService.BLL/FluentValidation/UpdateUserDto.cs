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
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+380\d{9}$")
                .WithMessage("Phone number must be in the format +380XXXXXXXXX")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Photo)
                .Must(BeAValidFile).WithMessage("Photo must be a valid image (.jpg, .jpeg, .png, .webp)")
                .Must(f => f.Length <= MaxFileSize).WithMessage("Photo size must be less than 5 MB")
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
