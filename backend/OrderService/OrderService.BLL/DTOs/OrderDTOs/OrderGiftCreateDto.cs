using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTOs.OrderDTOs
{
    public class OrderGiftCreateDto
    {
        public Guid GiftId { get; set; }
        public int Count { get; set; } = 1;
    }
}
