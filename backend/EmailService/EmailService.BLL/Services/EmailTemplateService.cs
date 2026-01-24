using EmailService.BLL.Service.Interfaces;
using System;

namespace EmailService.BLL.Service
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private const string PrimaryColor = "#e11d48";   
        private const string SoftBg = "#fff5f7";         
        private const string TextMain = "#1f2937";
        private const string TextMuted = "#6b7280";
        private const string BorderColor = "#fbcfe8";

        public string GetEmailConfirmationTemplate(string firstName, string confirmUrl)
        {
            return $"""
            <div style="background:{SoftBg}; padding:48px 16px; font-family: 'Segoe UI', Arial, sans-serif;">
                <div style="
                    max-width:520px;
                    margin:0 auto;
                    background:#ffffff;
                    border-radius:20px;
                    padding:40px 36px;
                    border:1px solid {BorderColor};
                ">
                    <h2 style="
                        margin:0 0 16px;
                        color:{PrimaryColor};
                        font-weight:600;
                        font-size:24px;
                        text-align:center;
                    ">
                        Підтвердження email
                    </h2>

                    <p style="color:{TextMain}; font-size:16px; margin:0 0 12px;">
                        Вітаємо, {firstName}
                    </p>

                    <p style="color:{TextMuted}; font-size:15px; line-height:1.6; margin:0 0 28px;">
                        Щоб завершити реєстрацію та активувати обліковий запис,
                        будь ласка, підтвердіть вашу електронну адресу.
                    </p>

                    <div style="text-align:center; margin:32px 0;">
                        <a href="{confirmUrl}" style="
                            display:inline-block;
                            padding:14px 32px;
                            background:{PrimaryColor};
                            color:#ffffff;
                            text-decoration:none;
                            border-radius:999px;
                            font-size:15px;
                            font-weight:500;
                            letter-spacing:0.2px;
                        ">
                            Підтвердити email
                        </a>
                    </div>

                    <p style="color:{TextMuted}; font-size:13px; line-height:1.5; margin:0;">
                        Якщо ви не створювали обліковий запис, просто проігноруйте цей лист.
                    </p>

                    <div style="
                        margin-top:36px;
                        padding-top:20px;
                        border-top:1px solid {BorderColor};
                        text-align:center;
                        font-size:14px;
                        color:{TextMuted};
                    ">
                        FlowerLab
                    </div>
                </div>
            </div>
            """;
        }

        public string GetPasswordResetTemplate(string firstName, string resetUrl)
        {
            return $"""
            <div style="background:{SoftBg}; padding:48px 16px; font-family: 'Segoe UI', Arial, sans-serif;">
                <div style="
                    max-width:520px;
                    margin:0 auto;
                    background:#ffffff;
                    border-radius:20px;
                    padding:40px 36px;
                    border:1px solid {BorderColor};
                ">
                    <h2 style="
                        margin:0 0 16px;
                        color:{PrimaryColor};
                        font-weight:600;
                        font-size:24px;
                        text-align:center;
                    ">
                        Скидання пароля
                    </h2>

                    <p style="color:{TextMain}; font-size:16px; margin:0 0 12px;">
                        Вітаємо, {firstName}
                    </p>

                    <p style="color:{TextMuted}; font-size:15px; line-height:1.6; margin:0 0 28px;">
                        Ми отримали запит на зміну пароля. Для створення нового пароля
                        натисніть кнопку нижче.
                    </p>

                    <div style="text-align:center; margin:32px 0;">
                        <a href="{resetUrl}" style="
                            display:inline-block;
                            padding:14px 32px;
                            background:{PrimaryColor};
                            color:#ffffff;
                            text-decoration:none;
                            border-radius:999px;
                            font-size:15px;
                            font-weight:500;
                            letter-spacing:0.2px;
                        ">
                            Змінити пароль
                        </a>
                    </div>

                    <p style="color:{TextMuted}; font-size:13px; line-height:1.5; margin:0;">
                        Якщо ви не надсилали запит — жодних дій виконувати не потрібно.
                    </p>

                    <div style="
                        margin-top:36px;
                        padding-top:20px;
                        border-top:1px solid {BorderColor};
                        text-align:center;
                        font-size:14px;
                        color:{TextMuted};
                    ">
                        FlowerLab
                    </div>
                </div>
            </div>
            """;
        }
    }
}