using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTOs.OrderDTOs
{
    public class OrderCreateDto
    {
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; } = null!;
        public string UserLastName { get; set; } = null!;
        public string? Notes { get; set; }
        public bool IsDelivery { get; set; }
        public string? GiftMessage { get; set; }
        public DeliveryInformationCreateDto? DeliveryInformation { get; set; }
        public IEnumerable<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();
        public IEnumerable<OrderGiftCreateDto> Gifts { get; set; } = new List<OrderGiftCreateDto>();
    }

}
