using Microsoft.AspNetCore.Mvc;
using OrderService.BLL.DTOs.OrderDTOs;
using OrderService.BLL.Services.Interfaces;
using OrderService.Domain.QueryParams;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedOrders([FromQuery] OrderSpecificationParameters parameters, CancellationToken cancellationToken)
        {
            var pagedOrders = await _orderService.GetPagedOrdersAsync(parameters, cancellationToken);
            return Ok(pagedOrders);
        }

        [HttpGet("{id:guid}", Name = "GetOrderById")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var order = await _orderService.GetByIdAsync(id, cancellationToken);
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto, CancellationToken cancellationToken)
        {
            var createdOrder = await _orderService.CreateAsync(dto, cancellationToken);
            return CreatedAtRoute("GetOrderById", new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] OrderUpdateDto dto, CancellationToken cancellationToken)
        {
            var updatedOrder = await _orderService.UpdateStatusAsync(id, dto, cancellationToken);
            return Ok(updatedOrder);
        }
    }
}
