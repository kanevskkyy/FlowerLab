using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTOs.GiftsDTOs
{
    public class GiftReadDto
    {
        public Guid Id { get; set; }
        public Dictionary<string, string> Name { get; set; } = new();
        public Dictionary<string, string>? Description { get; set; }
        public int AvailableCount { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
