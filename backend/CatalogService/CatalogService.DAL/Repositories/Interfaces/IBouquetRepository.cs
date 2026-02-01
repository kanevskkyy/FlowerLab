using CatalogService.DAL.Helpers;
using CatalogService.Domain.Entities;
using CatalogService.Domain.QueryParametrs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Interfaces
{
    public interface IBouquetRepository : IGenericRepository<Bouquet>
    {
        Task<(decimal minPrice, decimal maxPrice)> GetMinAndMaxPriceAsync(CancellationToken cancellationToken = default);
        Task<Bouquet?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedList<Bouquet>> GetBySpecificationPagedAsync(BouquetQueryParameters parameters, CancellationToken cancellationToken = default);
        Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
        void DeleteImages(IEnumerable<BouquetImage> images);
        void AddImage(BouquetImage image);
    }
}
