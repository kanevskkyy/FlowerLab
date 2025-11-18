using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class FlowerQuantityDto
    {
        public Guid FlowerId { get; set; }
        public int Quantity { get; set; }
    }
}
