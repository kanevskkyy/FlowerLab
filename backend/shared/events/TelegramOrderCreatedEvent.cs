using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.events
{
    public record TelegramOrderCreatedEvent
    {
        public Guid OrderId { get; init; }
        public string? CustomerName { get; init; } 
        public decimal TotalPrice { get; init; }
    }
}
