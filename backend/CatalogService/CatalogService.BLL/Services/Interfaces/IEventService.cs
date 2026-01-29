using CatalogService.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<EventDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<EventDto> CreateAsync(Dictionary<string, string> name, CancellationToken cancellationToken = default);
        Task<EventDto> UpdateAsync(Guid id, Dictionary<string, string> name, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
