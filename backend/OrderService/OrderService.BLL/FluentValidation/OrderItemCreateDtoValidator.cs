using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using OrderService.BLL.DTOs.OrderDTOs;

namespace OrderService.BLL.FluentValidation
{
    public class OrderItemCreateDtoValidator : AbstractValidator<OrderItemCreateDto>
    {
        public OrderItemCreateDtoValidator()
        {
            RuleFor(i => i.BouquetId)
                .NotEmpty().WithMessage("BouquetId є обов’язковим");

            RuleFor(i => i.Count)
                .GreaterThan(0).WithMessage("Кількість повинна бути більшою за 0");
        }
    }
}