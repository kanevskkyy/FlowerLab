using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.Repositories.Implementations
{
    public class GiftRepository : GenericRepository<Gift>, IGiftRepository
    {
        public GiftRepository(CatalogDbContext context) : base(context) { }

        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null)
        {
            return await _dbSet.AnyAsync(g => g.Name == name && (!excludeId.HasValue || g.Id != excludeId.Value));
        }
    }
}
