using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class BouquetSummaryDto
    {
        public Guid Id { get; set; }
        public Dictionary<string, string> Name { get; set; } = new();
        public decimal Price { get; set; } 
        public string MainPhotoUrl { get; set; } = null!;
        public List<BouquetSizeDto> Sizes { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
