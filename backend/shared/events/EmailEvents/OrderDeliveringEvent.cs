using System;

namespace shared.events.EmailEvents
{
    public class OrderDeliveringEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public string? UserFirstName { get; set; }
        public string? UserEmail { get; set; }
        public Guid OrderId { get; set; }
    }
}
