using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTOs.OrderDTOs
{
    public class OrderCreateResultDto
    {
        public OrderDetailDto Order { get; set; } = null!;
        public string PaymentUrl { get; set; } = string.Empty;
    }

}
