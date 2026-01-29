using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class BouquetSizeDto
    {
        public Guid SizeId { get; set; }
        public Dictionary<string, string> SizeName { get; set; } = new();
        public decimal Price { get; set; }
        public List<FlowerInBouquetDto> Flowers { get; set; } = new();
        public List<BouquetImageDto> Images { get; set; } = new();
        public int MaxAssemblableCount { get; set; }
        public bool IsAvailable { get; set; }
    }
}
