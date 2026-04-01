using EmailService.BLL.DTO;
using EmailService.BLL.Service.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using shared.events.EmailEvents;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EmailService.BLL.Consumers
{
    public class OrderPaidEventConsumer : IConsumer<OrderPaidEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;
        private readonly ILogger<OrderPaidEventConsumer> _logger;
        private readonly HttpClient _httpClient;

        public OrderPaidEventConsumer(
            IEmailService emailService,
            IEmailTemplateService templateService,
            ILogger<OrderPaidEventConsumer> logger,
            IHttpClientFactory httpClientFactory)
        {
            _emailService = emailService;
            _templateService = templateService;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("UsersServiceClient");
        }

        public async Task Consume(ConsumeContext<OrderPaidEvent> context)
        {
            var message = context.Message;
            string? email = message.UserEmail;

            // If email is missing, try to fetch it from UsersService
            if (string.IsNullOrEmpty(email) && message.UserId.HasValue)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"/api/me/internal/{message.UserId}/email");
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadFromJsonAsync<UserEmailResponse>();
                        email = data?.Email;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch email for UserId {UserId} during OrderPaid notification", message.UserId);
                }
            }

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("No email found for Order {OrderId}. Skipping paid notification.", message.OrderId);
                return;
            }

            foreach (var item in message.Items)
            {
                _logger.LogInformation("[Email Consumer Debug] Item {ItemName} received ImageUrl: {ImageUrl}", item.Name, item.ImageUrl);
            }

            try
            {
                var htmlContent = _templateService.GetOrderPaidTemplate(
                    message.UserFirstName ?? "Клієнт",
                    message.OrderId,
                    message.TotalPrice,
                    message.Subtotal,
                    message.DeliveryPrice,
                    message.DiscountAmount,
                    message.ShippingAddress ?? "Адреса магазину",
                    message.IsDelivery,
                    message.Items);

                await _emailService.SendEmailAsync(new EmailMessageDTO
                {
                    To = email,
                    Subject = "FlowerLab: Ваше замовлення оплачено - Дякуємо! ✨",
                    HtmlBody = htmlContent
                }, context.CancellationToken);

                _logger.LogInformation("Paid confirmation email sent to {Email} for order {OrderId}", email, message.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Paid confirmation email to {Email}", email);
            }
        }

        private class UserEmailResponse { public string? Email { get; set; } }
    }
}
