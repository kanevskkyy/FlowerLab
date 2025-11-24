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

            RuleFor(o => o.FirstName)
                .MaximumLength(100)
                .WithMessage("Ім'я не може перевищувати 100 символів");

            RuleFor(o => o.LastName)
                .MaximumLength(100)
                .WithMessage("Прізвище не може перевищувати 100 символів");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?\d{10,15}$").WithMessage("Номер телефону має бути валідним")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(o => o.Notes)
                .MaximumLength(500)
                .WithMessage("Примітки не можуть перевищувати 500 символів");

            RuleFor(o => o.GiftMessage)
                .MaximumLength(300)
                .WithMessage("Поздоровлення не може перевищувати 300 символів");

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
