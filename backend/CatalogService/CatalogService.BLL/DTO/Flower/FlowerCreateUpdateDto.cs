using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class FlowerCreateUpdateDto
    {
        public Dictionary<string, string> Name { get; set; } = new();
        public int Quantity { get; set; }
    }
}
