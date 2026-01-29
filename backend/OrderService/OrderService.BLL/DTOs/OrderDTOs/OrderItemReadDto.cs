using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTOs.OrderDTOs
{
    public class OrderItemReadDto
    {
        public Guid Id { get; set; }
        public Guid BouquetId { get; set; }
        public Dictionary<string, string> BouquetName { get; set; } = new();
        public string BouquetImage { get; set; } = null!;
        public Guid SizeId { get; set; }
        public Dictionary<string, string> SizeName { get; set; } = new();
        public decimal Price { get; set; }
        public int Count { get; set; }
    }

}
