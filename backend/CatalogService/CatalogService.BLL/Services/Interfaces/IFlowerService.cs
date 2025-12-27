using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.BLL.DTO;

namespace CatalogService.BLL.Services.Interfaces
{
    public interface IFlowerService
    {
        Task<IEnumerable<FlowerDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<FlowerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<FlowerDto> CreateAsync(FlowerCreateUpdateDto dto, CancellationToken cancellationToken = default);
        Task<FlowerDto> UpdateAsync(Guid id, FlowerCreateUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
