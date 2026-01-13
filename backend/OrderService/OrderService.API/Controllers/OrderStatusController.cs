using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.BLL.DTOs.OrderStatusDTOs;
using OrderService.BLL.Services.Interfaces;
using System.Threading;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/order-statuses")]
    public class OrderStatusController : ControllerBase
    {
        private readonly IOrderStatusService orderStatusService;

        public OrderStatusController(IOrderStatusService orderStatusService)
        {
            this.orderStatusService = orderStatusService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var statuses = await orderStatusService.GetAllAsync(cancellationToken);
            return Ok(statuses);
        }

        [HttpGet("{id:guid}", Name = "GetOrderStatusById")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var status = await orderStatusService.GetByIdAsync(id, cancellationToken);
            return Ok(status);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(
            [FromBody] OrderStatusCreateDto dto,
            CancellationToken cancellationToken)
        {
            var createdStatus = await orderStatusService.CreateAsync(dto, cancellationToken);
            return CreatedAtRoute(
                "GetOrderStatusById",
                new { id = createdStatus.Id },
                createdStatus);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(
            Guid id,
            [FromBody] OrderStatusUpdateDto dto,
            CancellationToken cancellationToken)
        {
            var updatedStatus = await orderStatusService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updatedStatus);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            await orderStatusService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
