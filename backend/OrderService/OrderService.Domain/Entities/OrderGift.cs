using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public class OrderGift
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public Guid GiftId { get; set; }
        public Gift Gift { get; set; } = null!;
        public int Count { get; set; } = 1;
    }
}
