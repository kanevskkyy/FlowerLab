using Grpc.Core;

namespace AggregatorService.Clients
{
    public class FilterGrpcClient : IFilterGrpcClient
    {
        public FilterService.FilterServiceClient filterServiceClient;
        private ILogger<FilterGrpcClient> logger;

        public FilterGrpcClient(FilterService.FilterServiceClient filterServiceClient, ILogger<FilterGrpcClient> logger)
        {
            this.filterServiceClient = filterServiceClient;
            this.logger = logger;
        }

        public async Task<FilterResponse> GetAllFiltersAsync()
        {
            try
            {
                var call = filterServiceClient.GetAllFiltersAsync(new FilterEmptyRequest());
                FilterResponse filterResponse = await call.ResponseAsync;
                return filterResponse;
            }
            catch (RpcException ex)
            {
                logger.LogError(ex, "gRPC error while fetching filters from CatalogService");
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while fetching filters from CatalogService");
                return null;
            }
        }
    }
}
