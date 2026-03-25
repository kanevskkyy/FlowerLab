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
        private LiqPaySettings settings;
        private LiqPayClient liqPayClient;

        public LiqPayService(IOptions<LiqPaySettings> options)
        {
            settings = options.Value;
            liqPayClient = new LiqPayClient(settings.PublicKey, settings.PrivateKey);
            liqPayClient.IsCnbSandbox = settings.IsSandbox;
        }

        public string GeneratePaymentUrl(Guid orderId, decimal amount, string description)
        {
            LiqPayRequest request = new LiqPayRequest
            {
                Action = LiqPayRequestAction.Pay,
                Amount = (double)amount,
                Currency = "UAH",
                Description = description,
                OrderId = $"{orderId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                ServerUrl = settings.ServerUrl,
                ResultUrl = $"{settings.SuccessUrl}?orderId={orderId}",
                Language = LiqPayRequestLanguage.UK
            };

            var result = liqPayClient.GenerateDataAndSignature(request);

            return $"https://www.liqpay.ua/api/3/checkout?data={result.Key}&signature={result.Value}";
        }

        public bool ValidateCallback(string data, string signature)
        {
            var expectedSignature = liqPayClient.CreateSignature(data);
            return expectedSignature == signature;
        }

        public LiqPayResponse ParseCallback(string data)
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(data));
            return JsonConvert.DeserializeObject<LiqPayResponse>(json);
        }

        public async Task<string> RefundPaymentAsync(string orderId, decimal amount)
        {
            var request = new LiqPayRequest
            {
                Action = LiqPayRequestAction.Refund,
                Amount = (double)amount,
                OrderId = orderId,
                Description = $"Refund for order {orderId}"
            };

            // Generate payload locally using the SDK to ensure correct signature
            var dataAndSig = liqPayClient.GenerateDataAndSignature(request);
            
            // Bypass the SDK's RequestAsync to prevent it from crashing while parsing LiqPayResponse.Action
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("data", dataAndSig.Key),
                new KeyValuePair<string, string>("signature", dataAndSig.Value)
            });

            var httpResponse = await client.PostAsync("https://www.liqpay.ua/api/request", content);
            var json = await httpResponse.Content.ReadAsStringAsync();
            
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            
            if (dict != null && dict.ContainsKey("status"))
            {
                var status = dict["status"].ToString();
                if (status == "error" && dict.ContainsKey("err_description"))
                {
                    return $"error ({dict["err_description"]})";
                }
                return status ?? "unknown";
            }
            
            return "unknown";
        }
    }
}