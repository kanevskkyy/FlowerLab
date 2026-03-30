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
    public class OrderReadyForPickupEventConsumer : IConsumer<OrderReadyForPickupEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILogger<OrderReadyForPickupEventConsumer> _logger;
        private readonly HttpClient _httpClient;

        public OrderReadyForPickupEventConsumer(
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            ILogger<OrderReadyForPickupEventConsumer> logger,
            IHttpClientFactory httpClientFactory)
        {
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("UsersServiceClient");
        }

        public async Task Consume(ConsumeContext<OrderReadyForPickupEvent> context)
        {
            var msg = context.Message;
            string? email = msg.UserEmail;

            _logger.LogInformation("Processing OrderReadyForPickupEvent for Order {OrderId}", msg.OrderId);

            // Option B: If email is missing (registered user), fetch it from UsersService
            if (string.IsNullOrEmpty(email) && msg.UserId.HasValue)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"api/me/internal/{msg.UserId}/email");
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadFromJsonAsync<UserEmailResponse>();
                        email = data?.Email;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch email for UserId {UserId} from UsersService", msg.UserId);
                }
            }

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("No email found for Order {OrderId}. Skipping notification.", msg.OrderId);
                return;
            }

            try
            {
                await _emailService.SendEmailAsync(new EmailMessageDTO
                {
                    To = email,
                    Subject = "FlowerLab: Ваше замовлення готове! ✨",
                    HtmlBody = _emailTemplateService.GetOrderReadyForPickupTemplate(
                        msg.UserFirstName ?? "Клієнт",
                        msg.OrderId,
                        msg.PickupAddress ?? "наш магазин")
                }, context.CancellationToken);

                _logger.LogInformation("Ready for Pickup notification sent to {Email} for Order {OrderId}", email, msg.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Ready for Pickup email to {Email}", email);
            }
        }

        private class UserEmailResponse { public string? Email { get; set; } }
    }
}
