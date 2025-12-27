using CatalogService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Interfaces
{
    public interface IRecipientRepository : IGenericRepository<Recipient>
    {
        Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    }
}
