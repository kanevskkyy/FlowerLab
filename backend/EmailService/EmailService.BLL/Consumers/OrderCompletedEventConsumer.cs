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
    public class OrderCompletedEventConsumer : IConsumer<OrderCompletedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;
        private readonly ILogger<OrderCompletedEventConsumer> _logger;
        private readonly HttpClient _httpClient;

        public OrderCompletedEventConsumer(
            IEmailService emailService,
            IEmailTemplateService templateService,
            ILogger<OrderCompletedEventConsumer> logger,
            IHttpClientFactory httpClientFactory)
        {
            _emailService = emailService;
            _templateService = templateService;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("UsersServiceClient");
        }

        public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
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
                    _logger.LogError(ex, "Failed to fetch email for UserId {UserId} during OrderCompleted notification", message.UserId);
                }
            }

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("No email found for Order {OrderId}. Skipping completion notification.", message.OrderId);
                return;
            }

            try
            {
                var htmlContent = _templateService.GetOrderCompletedTemplate(
                    message.UserFirstName ?? "Клієнт",
                    message.OrderId);

                await _emailService.SendEmailAsync(new EmailMessageDTO
                {
                    To = email,
                    Subject = "FlowerLab: Замовлення завершено! ✨",
                    HtmlBody = htmlContent
                }, context.CancellationToken);

                _logger.LogInformation("Order completed email sent to {Email} for order {OrderId}", email, message.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Order completed email to {Email}", email);
            }
        }

        private class UserEmailResponse { public string? Email { get; set; } }
    }
}
