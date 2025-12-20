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
        public string Name { get; set; } = null!;
        public decimal Price { get; set; } 
        public string MainPhotoUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
