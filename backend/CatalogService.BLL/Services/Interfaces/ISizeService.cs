using CatalogService.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Interfaces
{
    public interface ISizeService
    {
        Task<IEnumerable<SizeDto>> GetAllAsync();
        Task<SizeDto> GetByIdAsync(Guid id);
        Task<SizeDto> CreateAsync(string name);
        Task<SizeDto> UpdateAsync(Guid id, string name);
        Task DeleteAsync(Guid id);
    }
}
