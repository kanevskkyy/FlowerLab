using System;
using System.Collections.Generic;

namespace shared.events.EmailEvents
{
    public class OrderPaidEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Guid? UserId { get; set; }
        public string? UserFirstName { get; set; }
        public string? UserEmail { get; set; }
        public decimal TotalPrice { get; set; }
        public string? ShippingAddress { get; set; }
        public bool IsDelivery { get; set; }
        public List<OrderEmailItem> Items { get; set; } = new();
    }

    public class OrderEmailItem
    {
        public string? Name { get; set; }
        public string? Size { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
