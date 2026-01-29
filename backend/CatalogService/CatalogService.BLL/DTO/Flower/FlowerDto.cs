using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class FlowerDto
    {
        public Guid Id { get; set; }
        public Dictionary<string, string> Name { get; set; } = new();
        public int Quantity { get; set; }
    }
}
