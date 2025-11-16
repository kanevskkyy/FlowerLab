using CatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Interfaces
{
    public interface IRecipientRepository : IGenericRepository<Recipient>
    {
        Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null);
    }
}
