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
        Task<Bouquet?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedList<Bouquet>> GetBySpecificationPagedAsync(BouquetQueryParameters parameters, CancellationToken cancellationToken = default);
    }
}
