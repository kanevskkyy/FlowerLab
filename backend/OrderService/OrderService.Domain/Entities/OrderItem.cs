using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public Guid BouquetId { get; set; }
        public string BouquetName { get; set; } = null!;
        public string BouquetImage { get; set; } = null!;

        public Guid SizeId { get; set; }
        public string SizeName { get; set; } = null!;

        public decimal Price { get; set; }
        public int Count { get; set; }
        public ICollection<OrderItemFlower> Flowers { get; set; } = new List<OrderItemFlower>();
    }
}
