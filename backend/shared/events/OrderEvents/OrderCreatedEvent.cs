using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.events.OrderEvents
{
    public class OrderCreatedEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public List<OrderBouquetItem> Bouquets { get; set; } = new();
    }
}
