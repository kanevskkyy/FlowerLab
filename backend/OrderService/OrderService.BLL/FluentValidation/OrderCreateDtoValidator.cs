using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using OrderService.BLL.DTOs.OrderDTOs;

namespace OrderService.BLL.FluentValidation
{
    public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateDtoValidator()
        {
            RuleFor(o => o.UserId)
                .NotEmpty().WithMessage("UserId є обов’язковим");

            RuleFor(o => o.UserFirstName)
                .NotEmpty().WithMessage("Ім’я користувача є обов’язковим")
                .MaximumLength(50).WithMessage("Ім’я користувача не може перевищувати 50 символів");

            RuleFor(o => o.Notes)
                .MaximumLength(500)
                .WithMessage("Примітки не можуть перевищувати 500 символів");

            RuleFor(o => o.GiftMessage)
                .MaximumLength(300)
                .WithMessage("Поздоровлення не може перевищувати 300 символів");

            RuleFor(o => o.UserLastName)
                .NotEmpty().WithMessage("Прізвище користувача є обов’язковим")
                .MaximumLength(50).WithMessage("Прізвище користувача не може перевищувати 50 символів");

            RuleForEach(o => o.Items)
                .SetValidator(new OrderItemCreateDtoValidator());

            RuleForEach(o => o.Gifts)
                .SetValidator(new OrderGiftCreateDtoValidator());

            When(o => o.IsDelivery, () =>
            {
                RuleFor(o => o.DeliveryInformation)
                    .NotNull().WithMessage("Інформація про доставку є обов’язковою, якщо доставка увімкнена")
                    .SetValidator(new DeliveryInformationCreateDtoValidator());
            });
        }
    }
}
