using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public record EventDto
    {
        public Guid Id { get; init; }
        public Dictionary<string, string> Name { get; set; } = new();
    }

}
