using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OrderService.BLL.DTOs.GiftsDTOs
{
    public class GiftCreateDto
    {
            public Dictionary<string, string> Name { get; set; } = new();
        public Dictionary<string, string>? Description { get; set; }
        public IFormFile Image { get; set; } = null!;
        public int AvailableCount { get; set; }
        public decimal Price { get; set; }
    }

}
