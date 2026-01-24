using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.BLL.Service.Interfaces
{
    public interface IEmailTemplateService
    {
        string GetEmailConfirmationTemplate(string firstName, string confirmUrl);
        string GetPasswordResetTemplate(string firstName, string resetUrl);
    }
}
