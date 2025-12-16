using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using UsersService.BLL.Models.Users;

namespace UsersService.BLL.FluentValidation
{
    public class UpdateDiscountDtoValidator : AbstractValidator<UpdateDiscountDto>
    {
        public UpdateDiscountDtoValidator()
        {
            RuleFor(x => x.PersonalDiscountPercentage)
                .InclusiveBetween(0, 100)
                .WithMessage("Знижка повинна бути від 0 до 100.");
        }
    }
}
