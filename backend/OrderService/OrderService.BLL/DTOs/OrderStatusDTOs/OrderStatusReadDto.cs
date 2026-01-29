using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTOs.OrderStatusDTOs
{
    public class OrderStatusReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Dictionary<string, string> Translations { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
