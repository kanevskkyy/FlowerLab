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
        string GetOrderReadyForPickupTemplate(string firstName, Guid orderId, string pickupAddress);
        string GetOrderDeliveringTemplate(string firstName, Guid orderId);
        string GetOrderPaidTemplate(string firstName, Guid orderId, decimal totalPrice, string address, bool isDelivery, List<shared.events.EmailEvents.OrderEmailItem> items);
        string GetOrderCompletedTemplate(string firstName, Guid orderId);
    }
}
