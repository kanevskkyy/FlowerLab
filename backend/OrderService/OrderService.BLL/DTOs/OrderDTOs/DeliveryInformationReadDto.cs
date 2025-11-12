using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.DTOs.OrderDTOs
{
    public class DeliveryInformationReadDto
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = null!;
    }

}
