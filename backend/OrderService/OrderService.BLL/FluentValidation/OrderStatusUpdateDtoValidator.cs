using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using OrderService.BLL.DTOs.OrderStatusDTOs;

namespace OrderService.BLL.FluentValidation
{
    public class OrderStatusUpdateDtoValidator : AbstractValidator<OrderStatusUpdateDto>
    {
        public OrderStatusUpdateDtoValidator()
        {
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("Назва статусу є обов’язковою")
                .MaximumLength(50).WithMessage("Назва статусу не може перевищувати 50 символів");
        }
    }
}
