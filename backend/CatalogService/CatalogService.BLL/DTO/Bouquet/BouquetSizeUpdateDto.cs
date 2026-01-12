using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO.Bouquet
{
    public class BouquetSizeUpdateDto
    {
        public Guid SizeId { get; set; }
        public decimal Price { get; set; }
        public List<Guid> FlowerIds { get; set; } = new();
        public List<int> FlowerQuantities { get; set; } = new();
        public IFormFile? MainImage { get; set; }
        public List<IFormFile> NewImages { get; set; } = new();
    }

}
