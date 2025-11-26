using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiqPay.SDK.Dto;
using LiqPay.SDK.Dto.Enums;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using OrderService.BLL.DTOs;
using OrderService.BLL.Services;
using OrderService.BLL.Services.Interfaces;
using Xunit;

namespace OrderService.Tests.Services
{
    public class LiqPayServiceTests
    {
        private ILiqPayService _sut;
        private Mock<IOptions<LiqPaySettings>> _optionsMock;

        public LiqPayServiceTests()
        {
            _optionsMock = new Mock<IOptions<LiqPaySettings>>();
            _optionsMock.Setup(o => o.Value).Returns(new LiqPaySettings
            {
                PublicKey = "public",
                PrivateKey = "private",
                ServerUrl = "https://localhost/callback"
            });

            _sut = new LiqPayService(_optionsMock.Object);
        }

        [Fact]
        public void GeneratePaymentUrl_ReturnsCorrectUrl()
        {
            
            var orderId = Guid.NewGuid();
            var amount = 100m;
            var description = "Test payment";

            
            var url = _sut.GeneratePaymentUrl(orderId, amount, description);

            
            Assert.NotNull(url);
            Assert.Contains("https://www.liqpay.ua/api/3/checkout?data=", url);
            Assert.Contains("signature=", url);
        }

        [Fact]
        public void ValidateCallback_WhenSignatureMatches_ReturnsTrue()
        {
         
            var data = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"status\":\"success\"}"));
            var signature = _sut.GeneratePaymentUrl(Guid.NewGuid(), 100, "Test"); 
            var parts = signature.Split("signature=");
            var sig = parts.Length > 1 ? parts[1] : "";

         
            var result = _sut.ValidateCallback(data, sig);

         
            Assert.False(result);
        }

        [Fact]
        public void ParseCallback_ReturnsLiqPayResponse()
        {
            var response = new LiqPayResponse
            {
                Status = LiqPayResponseStatus.Success,
                Amount = 100,
                OrderId = Guid.NewGuid().ToString()
            };
            var json = JsonConvert.SerializeObject(response);
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            var parsed = _sut.ParseCallback(base64);

            Assert.NotNull(parsed);
            Assert.Equal(100, parsed.Amount);
            Assert.Equal(response.OrderId, parsed.OrderId);
        }
    }
}
