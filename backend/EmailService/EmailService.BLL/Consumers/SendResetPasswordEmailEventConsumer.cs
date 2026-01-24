using EmailService.BLL.DTO;
using EmailService.BLL.Service.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using shared.events.EmailEvents;
using System;
using System.Threading.Tasks;

namespace EmailService.BLL.Consumers
{
    public class SendResetPasswordEmailEventConsumer : IConsumer<UserResetPasswordEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILogger<SendResetPasswordEmailEventConsumer> _logger;

        public SendResetPasswordEmailEventConsumer(
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            ILogger<SendResetPasswordEmailEventConsumer> logger)
        {
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserResetPasswordEvent> context)
        {
            _logger.LogInformation(
                "Start processing UserResetPasswordEvent. Email: {Email}",
                context.Message.Email
            );

            try
            {
                await _emailService.SendEmailAsync(
                    new EmailMessageDTO
                    {
                        To = context.Message.Email,
                        Subject = "Reset password",
                        HtmlBody = _emailTemplateService.GetPasswordResetTemplate(
                            context.Message.FirstName!,
                            context.Message.ResetPasswordURL!)
                    },
                    context.CancellationToken
                );

                _logger.LogInformation(
                    "Reset password email successfully sent. Email: {Email}",
                    context.Message.Email
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while sending reset password email. Email: {Email}",
                    context.Message.Email
                );

                throw; 
            }
        }
    }
}
