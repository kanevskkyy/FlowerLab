using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid StatusId { get; set; }
        public OrderStatus Status { get; set; } = null!;

        public Guid? UserId { get; set; }
        public string? UserFirstName { get; set; } = null!;
        public string? UserLastName { get; set; } = null!;
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime? UpdatedAt { get; set; } 
        public bool IsDelivery { get; set; }
        public decimal TotalPrice { get; set; }
        public string? GiftMessage { get; set; }
        public string? PickupStoreAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? DeliveryInformationId { get; set; }
        public DeliveryInformation? DeliveryInformation { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<OrderGift> OrderGifts { get; set; } = new List<OrderGift>();
    }

}
