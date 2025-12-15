using shared.cache;

namespace AggregatorService.Redis
{
    public class FilterCacheInvalidationService : IEntityCacheInvalidationService<FilterResponse>
    {
        private readonly IEntityCacheService _cache;
        private readonly ILogger<FilterCacheInvalidationService> _logger;

        private const string CACHE_KEY = "filters:all";

        public FilterCacheInvalidationService(IEntityCacheService cache, ILogger<FilterCacheInvalidationService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task InvalidateByIdAsync(FilterResponse entity)
        {
            try
            {
                await _cache.RemoveAsync(CACHE_KEY);
                _logger.LogInformation("Кеш для фільтрів очищено по конкретному об’єкту.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не вдалося очистити кеш для фільтрів по об’єкту");
                throw;
            }
        }

        public async Task InvalidateAllAsync()
        {
            try
            {
                await _cache.RemoveByPatternAsync(CACHE_KEY + "*");
                _logger.LogInformation("Кеш для всіх фільтрів очищено.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не вдалося очистити кеш для всіх фільтрів");
                throw;
            }
        }

        public Task InvalidateByIdAsync(Guid entityId)
        {
            throw new NotImplementedException();
        }
    }
}
