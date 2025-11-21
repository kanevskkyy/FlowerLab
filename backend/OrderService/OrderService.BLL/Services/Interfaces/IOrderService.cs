using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.BLL.DTOs.OrderDTOs;
using OrderService.DAL.Helpers;
using OrderService.Domain.QueryParams;

namespace OrderService.BLL.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PagedList<OrderSummaryDto>> GetPagedOrdersAsync(OrderSpecificationParameters parameters, CancellationToken cancellationToken = default);
        Task<OrderDetailDto> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<OrderDetailDto> CreateAsync(Guid? userId,
            string userFirstName,
            string userLastName,
            OrderCreateDto dto,
            decimal personalDiscount,
            CancellationToken cancellationToken = default);
        Task<OrderDetailDto> UpdateStatusAsync(Guid orderId, OrderUpdateDto dto, CancellationToken cancellationToken = default);
    }
}
