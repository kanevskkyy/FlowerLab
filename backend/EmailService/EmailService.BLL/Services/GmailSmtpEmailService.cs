using EmailService.BLL.DTO;
using EmailService.BLL.Service.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EmailService.BLL.Service
{
    public class GmailSmtpEmailService : IEmailService
    {
        private readonly EmailProviderOptions _options;

        public GmailSmtpEmailService(IOptions<EmailProviderOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(EmailMessageDTO emailMessageDTO, CancellationToken cancellationToken = default)
        {
            using var smtpClient = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
            {
                Credentials = new NetworkCredential(_options.FromEmail, _options.SmtpPassword),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_options.FromEmail!, _options.FromName),
                Subject = emailMessageDTO.Subject,
                Body = emailMessageDTO.HtmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(emailMessageDTO.To);

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
        }
    }
}
