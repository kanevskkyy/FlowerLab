using OrderService.Domain.Entities;
using shared.cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.RedisCache
{
    public class GiftCacheInvalidationService : IEntityCacheInvalidationService<Gift>
    {
        private readonly IEntityCacheService cacheService;

        private const string ALL_KEY = "gifts:all";

        public GiftCacheInvalidationService(IEntityCacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public async Task InvalidateByIdAsync(Guid entityId)
        {
            await cacheService.RemoveAsync($"gifts:{entityId}");
        }

        public async Task InvalidateAllAsync()
        {
            await cacheService.RemoveAsync(ALL_KEY);
        }
    }
}
