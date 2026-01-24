using EmailService.BLL.DTO;
using EmailService.BLL.Service.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using shared.events.EmailEvents;
using System;
using System.Threading.Tasks;

namespace EmailService.BLL.Consumers
{
    public class SendVerifyEmailEventConsumer : IConsumer<UserRegisteredEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILogger<SendVerifyEmailEventConsumer> _logger;

        public SendVerifyEmailEventConsumer(
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            ILogger<SendVerifyEmailEventConsumer> logger)
        {
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            _logger.LogInformation(
                "Start processing UserRegisteredEvent. Email: {Email}",
                context.Message.UserEmail
            );

            try
            {
                await _emailService.SendEmailAsync(
                    new EmailMessageDTO
                    {
                        To = context.Message.UserEmail,
                        Subject = "Email Confirmation",
                        HtmlBody = _emailTemplateService.GetEmailConfirmationTemplate(
                            context.Message.UserFirstName!,
                            context.Message.ConfirmURL!)
                    },
                    context.CancellationToken
                );

                _logger.LogInformation(
                    "Verification email successfully sent. Email: {Email}",
                    context.Message.UserEmail
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while sending verification email. Email: {Email}",
                    context.Message.UserEmail
                );

                throw;
            }
        }
    }
}