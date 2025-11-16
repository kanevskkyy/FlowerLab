using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.BLL.DTOs.GiftsDTOs;
using OrderService.BLL.DTOs.OrderStatusDTOs;

namespace OrderService.BLL.DTOs.OrderDTOs
{
    public class OrderDetailDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; } = null!;
        public string UserLastName { get; set; } = null!;
        public string? Notes { get; set; }
        public bool IsDelivery { get; set; }
        public OrderStatusReadDto Status { get; set; } = null!;
        public DeliveryInformationReadDto? DeliveryInformation { get; set; }
        public IEnumerable<OrderItemReadDto> Items { get; set; } = new List<OrderItemReadDto>();
        public IEnumerable<GiftReadDto> Gifts { get; set; } = new List<GiftReadDto>();
        public decimal TotalPrice { get; set; }
    }

}
