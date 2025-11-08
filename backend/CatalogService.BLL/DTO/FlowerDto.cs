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
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string? Description { get; set; }
        public int Quantity { get; set; }
    }
}
