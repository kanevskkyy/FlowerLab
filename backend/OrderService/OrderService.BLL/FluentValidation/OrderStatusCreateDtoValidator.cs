using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using OrderService.BLL.DTOs.OrderStatusDTOs;

namespace OrderService.BLL.FluentValidation
{
    public class OrderStatusCreateDtoValidator : AbstractValidator<OrderStatusCreateDto>
    {
        public OrderStatusCreateDtoValidator()
        {
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("Назва статусу є обов’язковою")
                .MaximumLength(50);
        }
    }
}
