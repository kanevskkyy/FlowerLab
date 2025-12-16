namespace AggregatorService.Clients.Interfaces
{
    public interface IFilterGrpcClient
    {
        Task<FilterResponse> GetAllFiltersAsync();
    }
}
