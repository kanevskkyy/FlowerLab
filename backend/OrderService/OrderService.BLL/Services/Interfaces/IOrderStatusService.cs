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
        Task<IEnumerable<OrderStatusReadDto>> GetAllAsync();
        Task<OrderStatusReadDto> GetByIdAsync(Guid id);
        Task<OrderStatusReadDto> CreateAsync(OrderStatusCreateDto dto);
        Task<OrderStatusReadDto> UpdateAsync(Guid id, OrderStatusUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
