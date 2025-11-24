using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.BLL.DTOs.OrderStatusDTOs;

namespace OrderService.BLL.DTOs.OrderDTOs
{
    public class OrderSummaryDto
    {
        public Guid Id { get; set; }
        public string? PhoneNumber { get; set; }

        public string? UserFirstName { get; set; } 
        public string? UserLastName { get; set; }
        public string? PaymentUrl { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatusReadDto Status { get; set; } = null!;
    }

}
