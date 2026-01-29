using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public class Gift
    {
        public Guid Id { get; set; }
        public Dictionary<string, string> Name { get; set; } = new();
        public Dictionary<string, string>? Description { get; set; } = null!;
        public string ImageUrl { get; set; } = string.Empty;
        public int AvailableCount { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime? UpdatedAt { get; set; }

        public ICollection<OrderGift> OrderGifts { get; set; } = new List<OrderGift>();
    }
}
