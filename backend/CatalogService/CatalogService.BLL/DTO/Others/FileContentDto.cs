using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public class FileContentDto
    {
        public byte[] Content { get; set; } = null!;
        public string FileName { get; set; } = null!;
    }
}
