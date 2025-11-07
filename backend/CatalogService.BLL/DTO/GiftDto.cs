using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.DTO
{
    public record GiftDto(Guid Id, string GiftType, string Name, string? ImageUrl);

}
