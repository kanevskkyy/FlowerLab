using CatalogService.BLL.DTO.Bouquet;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class BouquetUpdateDto
    {
        public Dictionary<string, string> Name { get; set; } = new();
        public Dictionary<string, string>? Description { get; set; }
        public List<BouquetSizeUpdateDto> Sizes { get; set; } = new();
        public List<Guid> EventIds { get; set; } = new();
        public List<Guid> RecipientIds { get; set; } = new();
        public IFormFile? MainPhoto { get; set; }
    }
}
