using System;

namespace shared.events.EmailEvents
{
    public class OrderCompletedEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Guid? UserId { get; set; }
        public string? UserFirstName { get; set; }
        public String? UserEmail { get; set; }
    }
}
