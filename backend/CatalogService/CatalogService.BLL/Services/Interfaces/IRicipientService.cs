using CatalogService.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Interfaces
{
    public interface IRecipientService
    {
        Task<IEnumerable<RecipientDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<RecipientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<RecipientDto> CreateAsync(string name, CancellationToken cancellationToken = default);
        Task<RecipientDto> UpdateAsync(Guid id, string name, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
