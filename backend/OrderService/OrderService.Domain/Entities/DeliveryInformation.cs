using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public class DeliveryInformation
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = null!;
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
