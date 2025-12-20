using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.EmailService.DTO;
using UsersService.BLL.EmailService.Interfaces;

namespace UsersService.BLL.EmailService
{
    public class SendGridEmailService : IEmailService
    {
        private SendGridClient sendGridClient;
        private string fromEmail;
        private string fromName;

        public SendGridEmailService(IOptions<EmailProviderOptions> options)
        {
            sendGridClient = new SendGridClient(options.Value.ApiKey);
            fromEmail = options?.Value?.FromEmail;
            fromName = options?.Value?.FromName;
        }

        public async Task SendEmailAsync(EmailMessageDTO emailMessageDTO, CancellationToken cancellationToken = default)
        {
            SendGridMessage message = new SendGridMessage()
            {
                From = new EmailAddress(fromEmail, fromName),
                Subject = emailMessageDTO.Subject,
                PlainTextContent = null,
                HtmlContent = emailMessageDTO.HtmlBody
            };

            message.AddTo(emailMessageDTO.To);
            message.SetClickTracking(false, false);

            await sendGridClient.SendEmailAsync(message, cancellationToken);
        }
    }
}
