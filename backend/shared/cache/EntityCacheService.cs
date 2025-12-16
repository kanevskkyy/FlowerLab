using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace shared.cache
{
    public class EntityCacheService : IEntityCacheService, IDisposable
    {
        private readonly IMemoryCache memoryCache;
        private readonly IDatabase redisDb;
        private readonly ISubscriber subscriber;
        private readonly ILogger<EntityCacheService> logger;
        private const string INVALIDATION_CHANNEL = "entity-cache-invalidation";
        private const string CLEAR_ALL_MESSAGES = "__CLEAR_ALL__";
        private bool disposed;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = null,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };

        private readonly HashSet<string> _memoryKeys = new();

        public EntityCacheService(IMemoryCache memoryCache, IConnectionMultiplexer redisMultiplexer, ILogger<EntityCacheService> logger)
        {
            this.memoryCache = memoryCache;
            redisDb = redisMultiplexer.GetDatabase();
            this.logger = logger;

            subscriber = redisMultiplexer.GetSubscriber();
            subscriber.Subscribe(INVALIDATION_CHANNEL, (channel, message) =>
            {
                string msg = message.ToString();
                if (msg == CLEAR_ALL_MESSAGES) ClearAllMemoryCache();
                else RemoveFromMemory(msg);
            });
        }

        private void TrackMemoryKey(string key)
        {
            lock (_memoryKeys)
            {
                _memoryKeys.Add(key);
            }
        }

        private void RemoveFromMemory(string key)
        {
            memoryCache.Remove(key);
            lock (_memoryKeys)
            {
                _memoryKeys.Remove(key);
            }
        }

        private void ClearAllMemoryCache()
        {
            lock (_memoryKeys)
            {
                foreach (var key in _memoryKeys)
                {
                    memoryCache.Remove(key);
                }
                _memoryKeys.Clear();
            }
            logger.LogInformation("In-memory cache cleared successfully.");
        }

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? memoryExpiration = null, TimeSpan? redisExpiration = null)
        {
            if (memoryCache.TryGetValue<T>(key, out var memoryData))
            {
                logger.LogInformation("Cache hit: Memory | Key: {Key}", key);
                return memoryData;
            }

            try
            {
                var redisValue = await redisDb.StringGetAsync(key);
                if (redisValue.HasValue)
                {
                    var redisData = JsonSerializer.Deserialize<T>(redisValue, JsonOptions)!;
                    var memoryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = memoryExpiration ?? TimeSpan.FromMinutes(1),
                        Size = 1
                    };
                    memoryCache.Set(key, redisData, memoryOptions);
                    TrackMemoryKey(key);
                    logger.LogInformation("Cache hit: Redis | Key: {Key}", key);
                    return redisData;
                }
            }
            catch (RedisConnectionException ex)
            {
                logger.LogWarning(ex, "Redis unavailable, fetching from database | Key: {Key}", key);
            }

            logger.LogInformation("Cache miss | Key: {Key} — fetching from database", key);
            var dbData = await factory();
            if (dbData != null)
            {
                var memoryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = memoryExpiration ?? TimeSpan.FromMinutes(1),
                    Size = 1
                };
                memoryCache.Set(key, dbData, memoryOptions);
                TrackMemoryKey(key);
                try
                {
                    await redisDb.StringSetAsync(key, JsonSerializer.Serialize(dbData, JsonOptions), redisExpiration ?? TimeSpan.FromMinutes(5));
                }
                catch (RedisConnectionException ex)
                {
                    logger.LogWarning(ex, "Redis unavailable, skipping Redis cache | Key: {Key}", key);
                }
            }

            return dbData;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? memoryExpiration = null, TimeSpan? redisExpiration = null)
        {
            var memoryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = memoryExpiration ?? TimeSpan.FromMinutes(1),
                Size = 1
            };
            memoryCache.Set(key, value, memoryOptions);
            TrackMemoryKey(key);
            await redisDb.StringSetAsync(key, JsonSerializer.Serialize(value, JsonOptions), redisExpiration ?? TimeSpan.FromMinutes(5));
            await subscriber.PublishAsync(INVALIDATION_CHANNEL, key);
            logger.LogInformation("Cache set for key: {Key}", key);
        }

        public async Task RemoveAsync(string key)
        {
            RemoveFromMemory(key);
            await redisDb.KeyDeleteAsync(key);
            await subscriber.PublishAsync(INVALIDATION_CHANNEL, key);
            logger.LogInformation("Cache removed for key: {Key}", key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            var server = redisDb.Multiplexer.GetServer(redisDb.Multiplexer.GetEndPoints()[0]);
            foreach (var key in server.Keys(pattern: pattern + "*"))
            {
                await redisDb.KeyDeleteAsync(key);
            }
            await subscriber.PublishAsync(INVALIDATION_CHANNEL, CLEAR_ALL_MESSAGES);
            logger.LogInformation("Cache removed by pattern: {Pattern}", pattern);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                subscriber?.Unsubscribe(INVALIDATION_CHANNEL);
                disposed = true;
                logger.LogInformation("EntityCacheService disposed.");
            }
        }
    }
}