using CatalogService.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Interfaces
{
    public interface ISizeService
    {
        Task<IEnumerable<SizeDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SizeDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<SizeDto> CreateAsync(string name, CancellationToken cancellationToken = default);
        Task<SizeDto> UpdateAsync(Guid id, string name, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
