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
    public class OrderDeliveringEventConsumer : IConsumer<OrderDeliveringEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILogger<OrderDeliveringEventConsumer> _logger;
        private readonly HttpClient _httpClient;

        public OrderDeliveringEventConsumer(
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            ILogger<OrderDeliveringEventConsumer> logger,
            IHttpClientFactory httpClientFactory)
        {
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("UsersServiceClient");
        }

        public async Task Consume(ConsumeContext<OrderDeliveringEvent> context)
        {
            var msg = context.Message;
            string? email = msg.UserEmail;

            _logger.LogInformation("Processing OrderDeliveringEvent for Order {OrderId}", msg.OrderId);

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
                    Subject = "FlowerLab: Букет у дорозі! 📫",
                    HtmlBody = _emailTemplateService.GetOrderDeliveringTemplate(
                        msg.UserFirstName ?? "Клієнт",
                        msg.OrderId)
                }, context.CancellationToken);

                _logger.LogInformation("Delivering notification sent to {Email} for Order {OrderId}", email, msg.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Delivering email to {Email}", email);
            }
        }

        private class UserEmailResponse { public string? Email { get; set; } }
    }
}
