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
        public string Name { get; set; } = null!;
        public IFormFile Image { get; set; } = null!;
        public int AvailableCount { get; set; }
    }

}
