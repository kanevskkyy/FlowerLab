using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public record BouquetImageDto(
            Guid Id,
            string ImageUrl,
            short Position,
            bool IsMain
        );
}
