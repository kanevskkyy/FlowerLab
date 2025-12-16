using AggregatorService.Clients.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using shared.cache;
using System;
using System.Threading.Tasks;

namespace AggregatorService.Clients
{
    public class FilterGrpcClient : IFilterGrpcClient
    {
        private FilterService.FilterServiceClient _filterServiceClient;
        private ILogger<FilterGrpcClient> _logger;
        private IEntityCacheService _cache;
        private IEntityCacheInvalidationService<FilterResponse> _cacheInvalidation;

        private const string CACHE_KEY = "filters:all";

        public FilterGrpcClient(
            FilterService.FilterServiceClient filterServiceClient,
            ILogger<FilterGrpcClient> logger,
            IEntityCacheService cache,
            IEntityCacheInvalidationService<FilterResponse> cacheInvalidation)
        {
            _filterServiceClient = filterServiceClient;
            _logger = logger;
            _cache = cache;
            _cacheInvalidation = cacheInvalidation;
        }

        public async Task<FilterResponse?> GetAllFiltersAsync()
        {
            return await _cache.GetOrSetAsync(
                CACHE_KEY,
                async () =>
                {
                    try
                    {
                        var call = _filterServiceClient.GetAllFiltersAsync(new FilterEmptyRequest());
                        FilterResponse filterResponse = await call.ResponseAsync;

                        _logger.LogInformation("Filters fetched from CatalogService via gRPC.");
                        return filterResponse;
                    }
                    catch (RpcException ex)
                    {
                        _logger.LogError(ex, "gRPC error while fetching filters from CatalogService");
                        return null;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error while fetching filters from CatalogService");
                        return null;
                    }
                },
                memoryExpiration: TimeSpan.FromSeconds(30),
                redisExpiration: TimeSpan.FromMinutes(5)
            );
        }
    }
}