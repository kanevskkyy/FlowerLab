using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.events
{
    public record OrderAddressEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public string? Address { get; init; }
        public string? UserId { get; init; }
    }
}
