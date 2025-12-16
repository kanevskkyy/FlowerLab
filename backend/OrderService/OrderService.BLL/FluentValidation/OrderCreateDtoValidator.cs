using FluentValidation;
using OrderService.BLL.DTOs.OrderDTOs;
using OrderService.BLL.FluentValidation;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(o => o.FirstName)
            .NotEmpty()
            .WithMessage("Ім'я є обовʼязковим")
            .MaximumLength(100)
            .WithMessage("Ім'я не може перевищувати 100 символів");

        RuleFor(o => o.LastName)
            .NotEmpty()
            .WithMessage("Прізвище є обовʼязковим")
            .MaximumLength(100)
            .WithMessage("Прізвище не може перевищувати 100 символів");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Номер телефону є обовʼязковим")
            .Matches(@"^\+380\d{9}$")
            .WithMessage("Номер телефону має бути у форматі +380XXXXXXXXX");


        RuleFor(o => o.Notes)
            .MaximumLength(500)
            .WithMessage("Примітки не можуть перевищувати 500 символів");

        RuleFor(o => o.GiftMessage)
            .MaximumLength(300)
            .WithMessage("Поздоровлення не може перевищувати 300 символів");

        RuleForEach(o => o.Items)
            .SetValidator(new OrderItemCreateDtoValidator());

        RuleForEach(o => o.Gifts)
            .SetValidator(new OrderGiftCreateDtoValidator());

        When(o => o.IsDelivery, () =>
        {
            RuleFor(o => o.DeliveryInformation)
                .NotNull()
                .WithMessage("Інформація про доставку є обов’язковою, якщо доставка увімкнена")
                .SetValidator(new DeliveryInformationCreateDtoValidator());
        });

        When(o => !o.IsDelivery, () =>
        {
            RuleFor(o => o.PickupStoreAddress)
                .NotEmpty()
                .WithMessage("Адреса магазину обов’язкова при самовивозі");
        });
    }
}