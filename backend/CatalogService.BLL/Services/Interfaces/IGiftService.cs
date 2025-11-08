using CatalogService.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Interfaces
{
    public interface IGiftService
    {
        Task<IEnumerable<GiftDto>> GetAllAsync();
        Task<GiftDto> GetByIdAsync(Guid id);
        Task<GiftDto> CreateAsync(string name, string giftType, FileContentDto? image = null);
        Task<GiftDto> UpdateAsync(Guid id, string name, string giftType, FileContentDto? image = null);
        Task DeleteAsync(Guid id);
    }
}
