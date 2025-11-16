using CatalogService.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBouquetRepository Bouquets { get; }
        IFlowerRepository Flowers { get; }
        IEventRepository Events { get; }
        ISizeRepository Sizes { get; }
        IRecipientRepository Recipients { get; }

        Task<int> SaveChangesAsync();
    }
}
