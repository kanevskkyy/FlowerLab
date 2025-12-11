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
        public string BouquetName { get; set; } = null!;
        public string BouquetImage { get; set; } = null!;
        public Guid SizeId { get; set; }
        public string SizeName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Count { get; set; }
    }

}
