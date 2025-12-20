using FluentValidation;
using OrderService.BLL.DTOs.OrderDTOs;
using OrderService.BLL.FluentValidation;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(o => o.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters");

        RuleFor(o => o.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^\+380\d{9}$")
            .WithMessage("Phone number must be in the format +380XXXXXXXXX");

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
                .NotNull()
                .WithMessage("Delivery information is required if delivery is enabled")
                .SetValidator(new DeliveryInformationCreateDtoValidator());
        });

        When(o => !o.IsDelivery, () =>
        {
            RuleFor(o => o.PickupStoreAddress)
                .NotEmpty()
                .WithMessage("Store address is required for pickup");
        });
    }
}
