using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiqPay.SDK.Dto;

namespace OrderService.BLL.Services.Interfaces
{
    public interface ILiqPayService
    {
        string GeneratePaymentUrl(Guid orderId, decimal amount, string description);
        bool ValidateCallback(string data, string signature);
        LiqPayResponse ParseCallback(string data);
    }
}
