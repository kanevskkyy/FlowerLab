using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class BouquetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string MainPhotoUrl { get; set; } = null!;
        public List<BouquetSizeDto> Sizes { get; set; } = new();

        public List<EventDto> Events { get; set; } = new();
        public List<RecipientDto> Recipients { get; set; } = new();
        public List<BouquetImageDto> Images { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
