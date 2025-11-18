using CatalogService.Domain.Entities;
using CatalogService.Domain.QueryParametrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Interfaces
{
    public interface IBouquetRepository : IGenericRepository<Bouquet>
    {
        Task<IEnumerable<Bouquet>> GetBySpecificationAsync(BouquetQueryParameters parameters);
    }
}
