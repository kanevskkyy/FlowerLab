using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.BLL.DTOs.GiftsDTOs;

namespace OrderService.BLL.Services.Interfaces
{
    public interface IGiftService
    {
        Task<IEnumerable<GiftReadDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GiftReadDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<GiftReadDto> CreateAsync(GiftCreateDto dto, CancellationToken cancellationToken = default);
        Task<GiftReadDto> UpdateAsync(Guid id, GiftUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
