using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.BLL.DTOs.GiftsDTOs;

namespace OrderService.BLL.DTOs.OrderDTOs
{
    public class OrderGiftReadDto
    {
        public GiftReadDto? Gift {  get; set; }
        public int OrderedCount { get; set; }
    }
}
