using AggregatorService.DTOs;
using shared.cache;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AggregatorService.Redis
{
    public class FilterCacheInvalidationService : IEntityCacheInvalidationService<FilterResponseDto>, IEntityCacheInvalidationService<global::FilterResponse>
    {
        private IEntityCacheService _cache;
        private ILogger<FilterCacheInvalidationService> _logger;

        private const string CACHE_KEY = "filters:dto:all";

        public FilterCacheInvalidationService(IEntityCacheService cache, ILogger<FilterCacheInvalidationService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task InvalidateByIdAsync(FilterResponseDto entity)
        {
            try
            {
                await _cache.RemoveAsync(CACHE_KEY);
                _logger.LogInformation("Cache for filters cleared for specific entity.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear cache for filters by entity");
                throw;
            }
        }

        public async Task InvalidateByIdAsync(global::FilterResponse entity)
        {
            try
            {
                await _cache.RemoveAsync(CACHE_KEY);
                _logger.LogInformation("Cache for filters cleared for specific entity (Proto).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear cache for filters by entity (Proto)");
                throw;
            }
        }

        public async Task InvalidateAllAsync()
        {
            try
            {
                await _cache.RemoveByPatternAsync(CACHE_KEY + "*");
                _logger.LogInformation("Cache for all filters cleared.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear cache for all filters");
                throw;
            }
        }

        public Task InvalidateByIdAsync(Guid entityId)
        {
            throw new NotImplementedException();
        }
    }
}