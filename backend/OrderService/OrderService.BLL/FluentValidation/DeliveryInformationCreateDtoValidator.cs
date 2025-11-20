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
                .NotEmpty().WithMessage("Адреса є обов’язковою")
                .MaximumLength(200).WithMessage("Адреса не може перевищувати 200 символів");
        }
    }
}
