using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class FlowerCreateUpdateDto
    {
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
