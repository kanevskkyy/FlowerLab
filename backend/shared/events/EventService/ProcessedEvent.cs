using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.events.EventService
{
    public class ProcessedEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public Guid EventId { get; set; }              
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow.ToUniversalTime(); 
    }
}
