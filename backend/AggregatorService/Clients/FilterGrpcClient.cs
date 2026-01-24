using AggregatorService.Clients.Interfaces;
using AggregatorService.DTOs;
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
        private IEntityCacheInvalidationService<FilterResponseDto> _cacheInvalidation;

        private const string CACHE_KEY = "filters:all";

        public FilterGrpcClient(
            FilterService.FilterServiceClient filterServiceClient,
            ILogger<FilterGrpcClient> logger,
            IEntityCacheService cache,
            IEntityCacheInvalidationService<FilterResponseDto> cacheInvalidation)
        {
            _filterServiceClient = filterServiceClient;
            _logger = logger;
            _cache = cache;
            _cacheInvalidation = cacheInvalidation;
        }

        public async Task<FilterResponseDto?> GetAllFiltersAsync()
        {
            return await _cache.GetOrSetAsync(
                "filters:dto:all",
                async () =>
                {
                    try
                    {
                        var call = _filterServiceClient.GetAllFiltersAsync(new FilterEmptyRequest());
                        FilterResponse filters = await call.ResponseAsync;

                        _logger.LogInformation("Filters fetched from CatalogService via gRPC.");
                        
                        return new AggregatorService.DTOs.FilterResponseDto
                        {
                            PriceRange = filters.PriceRange != null ? new AggregatorService.DTOs.FilterPriceRangeDto
                            {
                                MinPrice = filters.PriceRange.MinPrice,
                                MaxPrice = filters.PriceRange.MaxPrice
                            } : null,
                            
                            SizeResponseList = filters.SizeResponseList?.Sizes?.Select(s => new AggregatorService.DTOs.FilterSizeDto 
                            {
                                Id = s.Id,
                                Name = s.Name
                            }).ToList() ?? new List<AggregatorService.DTOs.FilterSizeDto>(),

                            EventResponseList = filters.EventResponseList?.Events?.Select(e => new AggregatorService.DTOs.FilterEventDto 
                            {
                                Id = e.Id,
                                Name = e.Name
                            }).ToList() ?? new List<AggregatorService.DTOs.FilterEventDto>(),
                            
                            ReceivmentResponseList = filters.ReceivmentResponseList?.Receivments?.Select(r => new AggregatorService.DTOs.FilterRecipientDto 
                            {
                                Id = r.Id,
                                Name = r.Name
                            }).ToList() ?? new List<AggregatorService.DTOs.FilterRecipientDto>(),
                            
                            FlowerResponseList = filters.FlowerResponseList?.Flowers?.Select(f => new AggregatorService.DTOs.FilterFlowerDto 
                            {
                                Id = f.Id,
                                Name = f.Name,
                                Color = f.Color,
                                Description = f.Description,
                                Quantity = f.Quantity
                            }).ToList() ?? new List<AggregatorService.DTOs.FilterFlowerDto>()
                        };
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