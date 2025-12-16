using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NpgsqlTypes;
using OrderService.BLL.DTOs.OrderStatusDTOs;
using OrderService.BLL.Services.Interfaces;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/order-statuses")]
    public class OrderStatusController : ControllerBase
    {
        private IOrderStatusService orderStatusService;

        public OrderStatusController(IOrderStatusService orderStatusService)
        {
            this.orderStatusService = orderStatusService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAsync()
        {
            var statuses = await orderStatusService.GetAllAsync();
            return Ok(statuses);
        }

        [HttpGet("{id:guid}", Name = "GetOrderStatusById")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var status = await orderStatusService.GetByIdAsync(id);
            return Ok(status);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync([FromBody] OrderStatusCreateDto dto)
        {
            var createdStatus = await orderStatusService.CreateAsync(dto);
            return CreatedAtRoute("GetOrderStatusById", new { id = createdStatus.Id }, createdStatus);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] OrderStatusUpdateDto dto)
        {
            var updatedStatus = await orderStatusService.UpdateAsync(id, dto);
            return Ok(updatedStatus);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await orderStatusService.DeleteAsync(id);
            return NoContent();
        }
    }
}
