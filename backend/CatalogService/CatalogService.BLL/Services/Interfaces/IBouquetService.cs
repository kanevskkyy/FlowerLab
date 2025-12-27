using CatalogService.BLL.DTO;
using CatalogService.DAL.Helpers;
using CatalogService.Domain.QueryParametrs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Interfaces
{
    public interface IBouquetService
    {
        Task<PagedList<BouquetSummaryDto>> GetAllAsync(BouquetQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<BouquetDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<BouquetDto> CreateAsync(BouquetCreateDto dto, CancellationToken cancellationToken = default);
        Task<BouquetDto> UpdateAsync(Guid id, BouquetUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
