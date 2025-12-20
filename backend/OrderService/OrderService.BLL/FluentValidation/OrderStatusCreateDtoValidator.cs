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
                .NotEmpty()
                .WithMessage("Status name is required")
                .MaximumLength(50)
                .WithMessage("Status name cannot exceed 50 characters");
        }
    }
}
