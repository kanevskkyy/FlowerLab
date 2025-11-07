using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class BouquetCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Guid SizeId { get; set; } 
        public Guid? GiftId { get; set; }

        public List<FlowerQuantityDto> Flowers { get; set; } = new();

        public List<Guid> EventIds { get; set; } = new();
        public List<Guid> RecipientIds { get; set; } = new();

        public List<FileContentDto> Images { get; set; } = new();

        public FileContentDto? MainPhoto { get; set; }
    }
}
