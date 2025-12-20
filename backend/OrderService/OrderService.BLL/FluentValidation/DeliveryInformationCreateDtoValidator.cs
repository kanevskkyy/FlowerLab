using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using OrderService.BLL.DTOs.OrderDTOs;

namespace OrderService.BLL.FluentValidation
{
    public class DeliveryInformationCreateDtoValidator : AbstractValidator<DeliveryInformationCreateDto>
    {
        public DeliveryInformationCreateDtoValidator()
        {
            RuleFor(d => d.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters");
        }
    }
}
