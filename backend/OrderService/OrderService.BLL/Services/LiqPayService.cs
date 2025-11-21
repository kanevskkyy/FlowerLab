using LiqPay.SDK;
using LiqPay.SDK.Dto;
using LiqPay.SDK.Dto.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService.BLL.DTOs;
using OrderService.BLL.Services.Interfaces;
using System.Text;

namespace OrderService.BLL.Services
{
    public class LiqPayService : ILiqPayService
    {
        private readonly LiqPaySettings _settings;
        private readonly LiqPayClient _liqPayClient;

        public LiqPayService(IOptions<LiqPaySettings> options)
        {
            _settings = options.Value;
            _liqPayClient = new LiqPayClient(_settings.PublicKey, _settings.PrivateKey);
            _liqPayClient.IsCnbSandbox = true;
        }

        public string GeneratePaymentUrl(Guid orderId, decimal amount, string description)
        {
            var request = new LiqPayRequest
            {
                Action = LiqPayRequestAction.Pay,
                Amount = (double)amount,
                Currency = "UAH",
                Description = description,
                OrderId = orderId.ToString(),
                ServerUrl = _settings.ServerUrl,
                Language = LiqPayRequestLanguage.UK
            };

            var result = _liqPayClient.GenerateDataAndSignature(request);

            return $"https://www.liqpay.ua/api/3/checkout?data={result.Key}&signature={result.Value}";
        }

        public bool ValidateCallback(string data, string signature)
        {
            var expectedSignature = _liqPayClient.CreateSignature(data);
            return expectedSignature == signature;
        }

        public LiqPayResponse ParseCallback(string data)
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(data));
            return JsonConvert.DeserializeObject<LiqPayResponse>(json);
        }
    }
}