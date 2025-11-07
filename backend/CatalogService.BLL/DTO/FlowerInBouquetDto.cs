using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public record FlowerInBouquetDto(Guid Id, string Name, string Color, string Size, int Quantity);

}
