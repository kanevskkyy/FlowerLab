using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTOs.OrderDTOs
{
    public class OrderItemCreateDto
    {
        public Guid BouquetId { get; set; }
        public Guid SizeId { get; set; }
        public int Count { get; set; }
    }
}
