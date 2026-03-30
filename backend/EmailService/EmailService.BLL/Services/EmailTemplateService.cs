using EmailService.BLL.Service.Interfaces;
using System;

namespace EmailService.BLL.Service
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private const string PrimaryColor = "#f4bce5";   
        private const string SoftBg = "#fcf3f8";         
        private const string TextMain = "#463641";
        private const string TextMuted = "#7a6372";
        private const string BorderColor = "#fbeaf7";

        public string GetEmailConfirmationTemplate(string firstName, string confirmUrl)
        {
            return $"""
            <div style="background:{SoftBg}; padding:48px 16px; font-family: 'Manrope', 'Segoe UI', Arial, sans-serif;">
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
                        color:{TextMain};
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

                </div>
            </div>
            """;
        }

        public string GetPasswordResetTemplate(string firstName, string resetUrl)
        {
            return $"""
            <div style="background:{SoftBg}; padding:48px 16px; font-family: 'Manrope', 'Segoe UI', Arial, sans-serif;">
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
                        color:{TextMain};
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

                </div>
            </div>
            """;
        }

        public string GetOrderReadyForPickupTemplate(string firstName, Guid orderId, string pickupAddress)
        {
            return $"""
            <div style="background:{SoftBg}; padding:48px 16px; font-family: 'Manrope', 'Segoe UI', Arial, sans-serif;">
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
                        color:{TextMain};
                        font-weight:600;
                        font-size:24px;
                        text-align:center;
                    ">
                        Замовлення готове!
                    </h2>

                    <p style="color:{TextMain}; font-size:16px; margin:0 0 12px;">
                        Вітаємо, {firstName}
                    </p>

                    <p style="color:{TextMuted}; font-size:15px; line-height:1.6; margin:0 0 28px;">
                        Ваше замовлення <b>№{orderId.ToString().Substring(0, 8).ToUpper()}</b> успішно зібрано і вже чекає на вас!
                    </p>

                    <div style="
                        background:{SoftBg};
                        border-radius:12px;
                        padding:20px;
                        margin:24px 0;
                        border:1px dashed {PrimaryColor};
                    ">
                        <p style="margin:0 0 8px; color:{TextMain}; font-weight:600; font-size:14px;">
                            Адреса для самовивозу:
                        </p>
                        <p style="margin:0; color:{TextMain}; font-size:15px;">
                            {pickupAddress}
                        </p>
                    </div>

                    <p style="color:{TextMuted}; font-size:14px; line-height:1.5; margin:0 0 24px;">
                        Ми працюємо щодня. Будемо раді бачити вас у нашому магазині!
                    </p>

                </div>
            </div>
            """;
        }

        public string GetOrderDeliveringTemplate(string firstName, Guid orderId)
        {
            return $"""
            <div style="background:{SoftBg}; padding:48px 16px; font-family: 'Manrope', 'Segoe UI', Arial, sans-serif;">
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
                        color:{TextMain};
                        font-weight:600;
                        font-size:24px;
                        text-align:center;
                    ">
                        Букет у дорозі!
                    </h2>

                    <p style="color:{TextMain}; font-size:16px; margin:0 0 12px;">
                        Вітаємо, {firstName}
                    </p>

                    <p style="color:{TextMuted}; font-size:15px; line-height:1.6; margin:0 0 28px;">
                        Ваше замовлення <b>№{orderId.ToString().Substring(0, 8).ToUpper()}</b> передано кур'єру і вже прямує до отримувача.
                    </p>

                    <p style="color:{TextMuted}; font-size:14px; line-height:1.5; margin:0 0 24px;">
                        Ми повідомимо вас, як тільки доставка буде завершена. Дякуємо, що обираєте FlowerLab!
                    </p>

                </div>
            </div>
            """;
        }

        public string GetOrderPaidTemplate(string firstName, Guid orderId, decimal totalPrice, string address, bool isDelivery, List<shared.events.EmailEvents.OrderEmailItem> items)
        {
            var itemsHtml = string.Join("", items.Select(item => $"""
                <tr>
                    <td style="padding: 12px 0; border-bottom: 1px solid {BorderColor};">
                        <table style="width:100%; border-collapse:collapse;">
                            <tr>
                                <td style="width:50px; vertical-align:middle; padding-right:12px;">
                                    <img src="{item.ImageUrl ?? "https://placehold.co/100x100?text=Flower"}" 
                                         alt="{item.Name}" 
                                         width="50" 
                                         height="50"
                                         style="width: 50px; height: 50px; border-radius: 8px; display:block; object-fit: contain;" />
                                </td>
                                <td style="vertical-align:middle;">
                                    <div style="font-weight: 500; font-size: 14px; color: {TextMain}; line-height:1.2;">{item.Name}</div>
                                    <div style="font-size: 12px; color: {TextMuted}; margin-top:2px;">
                                        {(string.IsNullOrEmpty(item.Size) ? "" : item.Size + " • ")}{item.Count} шт.
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="padding: 12px 0; border-bottom: 1px solid {BorderColor}; text-align: right; font-weight: 500; font-size: 14px; color: {TextMain};">
                        {item.Price * item.Count:F2} ₴
                    </td>
                </tr>
            """));

            return $"""
            <div style="background:{SoftBg}; padding:48px 16px; font-family: 'Manrope', 'Segoe UI', Arial, sans-serif;">
                <div style="
                    max-width:520px;
                    margin:0 auto;
                    background:#ffffff;
                    border-radius:20px;
                    padding:40px 36px;
                    border:1px solid {BorderColor};
                ">
                    <div style="text-align:center; margin-bottom:24px;">
                        <span style="background:{SoftBg}; color:{PrimaryColor}; padding:8px 16px; border-radius:999px; font-size:12px; font-weight:600; text-transform:uppercase; letter-spacing:0.5px;">
                            Замовлення оплачено
                        </span>
                    </div>

                    <h2 style="margin:0 0 8px; color:{TextMain}; font-weight:700; font-size:24px; text-align:center;">
                        Дякуємо за покупку, {firstName}!
                    </h2>
                    <p style="color:{TextMuted}; font-size:15px; text-align:center; margin-bottom:32px;">
                        Ми отримали вашу оплату за замовлення <b>#{orderId.ToString().Substring(0, 8).ToUpper()}</b>.
                    </p>

                    <div style="margin-bottom:32px;">
                        <table style="width:100%; border-collapse:collapse;">
                            <thead>
                                <tr>
                                    <th style="text-align:left; font-size:12px; font-weight:600; color:{TextMuted}; text-transform:uppercase; padding-bottom:12px; border-bottom:2px solid {SoftBg};">Товар</th>
                                    <th style="text-align:right; font-size:12px; font-weight:600; color:{TextMuted}; text-transform:uppercase; padding-bottom:12px; border-bottom:2px solid {SoftBg};">Сума</th>
                                </tr>
                            </thead>
                            <tbody>
                                {itemsHtml}
                            </tbody>
                            <tfoot>
                                <tr>
                                    <td style="padding-top:20px; font-weight:600; font-size:16px; color:{TextMain};">Разом</td>
                                    <td style="padding-top:20px; text-align:right; font-weight:700; font-size:20px; color:{PrimaryColor};">
                                        {totalPrice:F2} ₴
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>

                    <div style="background:#fdfbfb; border-radius:16px; padding:20px; margin-bottom:32px; border:1px solid {BorderColor};">
                        <p style="margin:0 0 4px; color:{TextMuted}; font-size:12px; font-weight:600; text-transform:uppercase;">
                            {(isDelivery ? "Адреса доставки" : "Адреса самовивозу")}
                        </p>
                        <p style="margin:0; color:{TextMain}; font-size:15px; font-weight:500;">
                            {address}
                        </p>
                    </div>

                    <div style="text-align:center;">
                        <p style="color:{TextMuted}; font-size:14px; margin-bottom:24px;">
                            Ви можете відстежувати статус вашого замовлення в особистому кабінеті.
                        </p>
                    </div>

                </div>
            </div>
            """;
        }

        public string GetOrderCompletedTemplate(string firstName, Guid orderId)
        {
            return $"""
            <div style="background:{SoftBg}; padding:48px 16px; font-family: 'Manrope', 'Segoe UI', Arial, sans-serif;">
                <div style="
                    max-width:520px;
                    margin:0 auto;
                    background:#ffffff;
                    border-radius:20px;
                    padding:40px 36px;
                    border:1px solid {BorderColor};
                    text-align:center;
                ">
                    <h2 style="margin:0 0 16px; color:{TextMain}; font-weight:700; font-size:24px;">
                        Замовлення завершено!
                    </h2>
                    <p style="color:{TextMain}; font-size:16px; margin-bottom:12px;">
                        {firstName}, ваше замовлення №{orderId.ToString().Substring(0, 8).ToUpper()} виконано.
                    </p>
                    <p style="color:{TextMuted}; font-size:15px; line-height:1.6; margin-bottom:32px;">
                        Дякуємо, що довірили нам створення цього особливого моменту! Сподіваємось, наші квіти подарували багато радості.
                    </p>

                    <div style="margin:32px 0;">
                        <a href="https://flowerlab-vlada.com.ua" style="display:inline-block; padding:14px 32px; background:{PrimaryColor}; color:#ffffff; text-decoration:none; border-radius:999px; font-size:15px; font-weight:600; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);">
                            Замовити знову
                        </a>
                    </div>

                    <p style="color:{TextMuted}; font-size:14px;">
                        Ми будемо вдячні за ваш відгук у соцмережах!
                    </p>

                </div>
            </div>
            """;
        }
    }
}