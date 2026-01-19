using CatalogService.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking; // Added this using statement for EntityEntry

namespace CatalogService.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBouquetRepository Bouquets { get; }
        IFlowerRepository Flowers { get; }
        ISizeRepository Sizes { get; }
        IEventRepository Events { get; }
        IRecipientRepository Recipients { get; }
        
        IEnumerable<EntityEntry> GetChangeTrackerEntries();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
