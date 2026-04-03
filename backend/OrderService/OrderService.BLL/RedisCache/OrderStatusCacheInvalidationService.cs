using OrderService.Domain.Entities;
using shared.cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.RedisCache
{
    public class OrderStatusCacheInvalidationService : IEntityCacheInvalidationService<OrderStatus>
    {
        private readonly IEntityCacheService cacheService;

        private const string ALL_KEY = "order-status:all:v2";

        public OrderStatusCacheInvalidationService(IEntityCacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public async Task InvalidateByIdAsync(Guid entityId)
        {
            await cacheService.RemoveAsync($"order-status:{entityId}");
        }

        public async Task InvalidateAllAsync()
        {
            await cacheService.RemoveAsync(ALL_KEY);
        }
    }
}
