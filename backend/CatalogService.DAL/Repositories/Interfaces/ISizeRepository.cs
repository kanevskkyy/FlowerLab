using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.Domain.Entities;

namespace CatalogService.DAL.Repositories.Interfaces
{
    public interface ISizeRepository : IGenericRepository<Size>
    {
        Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null);
    }
}
