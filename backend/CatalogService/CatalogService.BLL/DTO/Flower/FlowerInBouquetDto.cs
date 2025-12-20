using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class FlowerInBouquetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public int Quantity { get; set; }

        public FlowerInBouquetDto() { }
    }
}
