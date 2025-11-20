using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using OrderService.BLL.DTOs.OrderDTOs;

namespace OrderService.BLL.FluentValidation
{
    public class OrderGiftCreateDtoValidator : AbstractValidator<OrderGiftCreateDto>
    {
        public OrderGiftCreateDtoValidator()
        {
            RuleFor(g => g.GiftId)
                .NotEmpty().WithMessage("GiftId є обов’язковим");

            RuleFor(g => g.Count)
                .GreaterThan(0).WithMessage("Кількість подарунків повинна бути більшою за 0");
        }
    }
}
