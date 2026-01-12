using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public class OrderItemFlower
    {
        public Guid OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; } = null!;

        public Guid FlowerId { get; set; }
        public string FlowerName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
