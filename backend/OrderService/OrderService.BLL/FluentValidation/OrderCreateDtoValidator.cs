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
            RuleFor(o => o.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters");

            RuleFor(o => o.GiftMessage)
                .MaximumLength(300)
                .WithMessage("Gift message cannot exceed 300 characters");

            RuleForEach(o => o.Items)
                .SetValidator(new OrderItemCreateDtoValidator());

            RuleForEach(o => o.Gifts)
                .SetValidator(new OrderGiftCreateDtoValidator());

            When(o => o.IsDelivery, () =>
            {
                RuleFor(o => o.DeliveryInformation)
                    .NotNull().WithMessage("Delivery information is required when delivery is enabled")
                    .SetValidator(new DeliveryInformationCreateDtoValidator());
            });
        }
    }
}
