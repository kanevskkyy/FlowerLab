namespace AggregatorService.Clients
{
    public interface IFilterGrpcClient
    {
        Task<FilterResponse> GetAllFiltersAsync();
    }
}
