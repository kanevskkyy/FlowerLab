using AggregatorService.DTOs;

namespace AggregatorService.Clients.Interfaces
{
    public interface IFilterGrpcClient
    {
        Task<FilterResponseDto?> GetAllFiltersAsync();
    }
}
