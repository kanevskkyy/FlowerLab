using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrderService.BLL.DTOs.OrderDTOs;
using OrderService.BLL.Services.Interfaces;
using OrderService.Domain.QueryParams;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin")]
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
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = string.IsNullOrEmpty(userIdString) ? null : Guid.Parse(userIdString);

            var firstName = User.FindFirstValue(ClaimTypes.GivenName);
            var lastName = User.FindFirstValue(ClaimTypes.Surname);

            dto.UserId = userId;
            dto.FirstName ??= firstName;
            dto.LastName ??= lastName;

            var result = await _orderService.CreateAsync(
                userId,
                firstName,
                lastName,
                dto,
                personalDiscount: decimal.Parse(User.FindFirstValue("Discount") ?? "0"),
                cancellationToken);

            return CreatedAtRoute("GetOrderById", new { id = result.Order.Id }, result);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders([FromQuery] OrderSpecificationParameters parameters, CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

            var userId = Guid.Parse(userIdString);
            var pagedOrders = await _orderService.GetMyOrdersAsync(userId, parameters, cancellationToken);
            return Ok(pagedOrders);
        }


        [HttpPost("liqpay-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> LiqPayCallback([FromForm] string data, [FromForm] string signature, CancellationToken cancellationToken)
        {
            await _orderService.ProcessPaymentCallbackAsync(data, signature, cancellationToken);
            return Ok();
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] OrderUpdateDto dto, CancellationToken cancellationToken)
        {
            var updatedOrder = await _orderService.UpdateStatusAsync(id, dto, cancellationToken);
            return Ok(updatedOrder);
        }
    }
}