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
        public string SizeName { get; set; } = null!;
        public decimal Price { get; set; } 
        public List<FlowerInBouquetDto> Flowers { get; set; } = new();
        public int MaxAssemblableCount { get; set; }
        public bool IsAvailable { get; set; }
    }
}
