using CatalogService.BLL.DTO;
using CatalogService.DAL.Helpers;
using CatalogService.Domain.QueryParametrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Interfaces
{
    public interface IBouquetService
    {
        Task<PagedList<BouquetSummaryDto>> GetAllAsync(BouquetQueryParameters parameters);
        Task<BouquetDto> GetByIdAsync(Guid id);
        Task<BouquetDto> CreateAsync(BouquetCreateDto dto);
        Task<BouquetDto> UpdateAsync(Guid id, BouquetUpdateDto dto);
        Task DeleteAsync(Guid id);

        Task UpdateFlowerQuantityAsync(Guid flowerId, int quantity);
    }
}
