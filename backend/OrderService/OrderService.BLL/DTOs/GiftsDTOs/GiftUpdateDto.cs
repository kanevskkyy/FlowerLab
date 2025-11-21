using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OrderService.BLL.DTOs.GiftsDTOs
{
    public class GiftUpdateDto
    {
        public string Name { get; set; } = null!;
        public int AvailableCount { get; set; }
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; } 
    }
}
