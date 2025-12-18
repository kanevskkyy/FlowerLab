using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public class GiftReservation
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public Guid GiftId { get; set; }
        public Gift Gift { get; set; }

        public int Quantity { get; set; }

        public DateTime ReservedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public bool IsActive { get; set; }
    }
}
