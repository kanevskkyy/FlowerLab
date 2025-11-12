using CatalogService.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllAsync();
        Task<EventDto> GetByIdAsync(Guid id);
        Task<EventDto> CreateAsync(string name);
        Task<EventDto> UpdateAsync(Guid id, string name);
        Task DeleteAsync(Guid id);
    }
}
