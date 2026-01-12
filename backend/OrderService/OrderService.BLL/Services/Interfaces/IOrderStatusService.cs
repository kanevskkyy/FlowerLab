using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.BLL.DTOs.OrderStatusDTOs;

namespace OrderService.BLL.Services.Interfaces
{
    public interface IOrderStatusService
    {
        Task<IEnumerable<OrderStatusReadDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<OrderStatusReadDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<OrderStatusReadDto> CreateAsync(OrderStatusCreateDto dto, CancellationToken cancellationToken = default);
        Task<OrderStatusReadDto> UpdateAsync(Guid id, OrderStatusUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
