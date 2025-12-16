using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.EmailService.DTO;

namespace UsersService.BLL.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessageDTO emailMessageDTO, CancellationToken cancellationToken = default);
    }
}
