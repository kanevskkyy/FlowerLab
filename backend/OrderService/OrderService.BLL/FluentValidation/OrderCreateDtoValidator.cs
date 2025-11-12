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
                .NotEmpty().WithMessage("UserId is required");

            RuleFor(o => o.UserFirstName)
                .NotEmpty().WithMessage("User first name is required")
                .MaximumLength(50).WithMessage("User first name cannot exceed 50 characters");

            RuleFor(o => o.UserLastName)
                .NotEmpty().WithMessage("User last name is required")
                .MaximumLength(50).WithMessage("User last name cannot exceed 50 characters");

            RuleForEach(o => o.Items)
                .SetValidator(new OrderItemCreateDtoValidator());

            When(o => o.IsDelivery, () =>
            {
                RuleFor(o => o.DeliveryInformation)
                    .NotNull().WithMessage("Delivery information is required when delivery is enabled")
                    .SetValidator(new DeliveryInformationCreateDtoValidator());
            });
        }
    }
}
