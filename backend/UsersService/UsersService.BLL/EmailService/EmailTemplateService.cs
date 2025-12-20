using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.BLL.EmailService.Interfaces;

namespace UsersService.BLL.EmailService
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string GetEmailConfirmationTemplate(string firstName, string confirmUrl)
        {
            return $"""
            <h2>Підтвердження email</h2>
            <p>Привіт, {firstName} 👋</p>
            <p>Натисни кнопку, щоб підтвердити email:</p>
            <a href="{confirmUrl}">
                Підтвердити email
            </a>
        """;
        }

        public string GetPasswordResetTemplate(string firstName, string resetUrl)
        {
            return $"""
            <h2>Скидання пароля</h2>
            <p>Привіт, {firstName} 👋</p>
            <p>Натисни кнопку, щоб змінити пароль:</p>
            <a href="{resetUrl}">
                Змінити пароль
            </a>
        """;
        }
    }
}
