using CatalogService.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Interfaces
{
    public interface IRecipientService
    {
        Task<IEnumerable<RecipientDto>> GetAllAsync();
        Task<RecipientDto> GetByIdAsync(Guid id);
        Task<RecipientDto> CreateAsync(string name);
        Task<RecipientDto> UpdateAsync(Guid id, string name);
        Task DeleteAsync(Guid id);
    }
}
