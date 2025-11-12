using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CatalogDbContext _context;

        public UnitOfWork(
            CatalogDbContext context,
            IBouquetRepository bouquetRepository,
            IFlowerRepository flowerRepository,
            IEventRepository eventRepository,
            ISizeRepository sizeRepository,
            IRecipientRepository recipientRepository
            )
        {
            _context = context;
            Bouquets = bouquetRepository;
            Flowers = flowerRepository;
            Events = eventRepository;
            Sizes = sizeRepository;
            Recipients = recipientRepository;
            
        }

        public IBouquetRepository Bouquets { get; }
        public IFlowerRepository Flowers { get; }
        public IEventRepository Events { get; }
        public ISizeRepository Sizes { get; }
        public IRecipientRepository Recipients { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
