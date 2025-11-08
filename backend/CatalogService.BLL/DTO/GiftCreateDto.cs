using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class GiftCreateDto
    {
        public string Name { get; set; } = null!;
        public string GiftType { get; set; } = null!;
        public IFormFile? Image { get; set; }
    }
}
