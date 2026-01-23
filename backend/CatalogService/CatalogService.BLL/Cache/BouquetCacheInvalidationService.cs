using CatalogService.Domain.Entities;
using shared.cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Cache
{
    public class BouquetCacheInvalidationService : IEntityCacheInvalidationService<Bouquet>
    {
        private IEntityCacheService _cache;

        public BouquetCacheInvalidationService(IEntityCacheService cache)
        {
            _cache = cache;
        }

        public async Task InvalidateByIdAsync(Guid bouquetId)
        {
            await _cache.RemoveAsync($"bouquet:{bouquetId}");
            await _cache.RemoveByPatternAsync("bouquets:");
            await _cache.RemoveByPatternAsync("bouquets_v2:");
        }

        public async Task InvalidateAllAsync()
        {
            await _cache.RemoveByPatternAsync("bouquets:");
            await _cache.RemoveByPatternAsync("bouquets_v2:");
        }
    }
}
