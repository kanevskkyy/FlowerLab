using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrderService.BLL.DTOs.OrderDTOs;
using OrderService.BLL.Services.Interfaces;
using OrderService.Domain.QueryParams;
using Microsoft.AspNetCore.Authorization;
using OrderService.DAL.Helpers;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class OrderController : ControllerBase
    {
        private IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPagedOrders([FromQuery] OrderSpecificationParameters parameters, CancellationToken cancellationToken)
        {
            var pagedOrders = await orderService.GetPagedOrdersAsync(parameters, cancellationToken);
            return Ok(pagedOrders);
        }

        [HttpGet("{id:guid}", Name = "GetOrderById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, [FromQuery] Guid? guestToken, CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            Guid? userId = string.IsNullOrEmpty(userIdString) ? null : Guid.Parse(userIdString);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var order = await orderService.GetByIdAsync(id, guestToken, cancellationToken);

            if (role != "Admin" && order.UserId != userId && order.GuestToken != guestToken)
                return Forbid();

            return Ok(order);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto, CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            Guid? userId = string.IsNullOrEmpty(userIdString) ? null : Guid.Parse(userIdString);
            var firstName = User.FindFirstValue(ClaimTypes.GivenName);
            var lastName = User.FindFirstValue(ClaimTypes.Surname);
            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var photoUrl = User.FindFirstValue("PhotoUrl");

            dto.FirstName ??= firstName;
            dto.LastName ??= lastName;
            dto.PhoneNumber ??= phoneNumber; 

            var result = await orderService.CreateAsync(
                userId,
                firstName,
                lastName,
                phoneNumber, 
                photoUrl,
                dto,
                personalDiscount: decimal.Parse(User.FindFirstValue("Discount") ?? "0"),
                cancellationToken);

            return CreatedAtRoute("GetOrderById", new { id = result.Id }, result);
        }

        [HttpGet("discount-eligibility")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckDiscountEligibility(CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            Console.WriteLine($"[CheckDiscount] User ID String: '{userIdString}'"); 
            
            if (string.IsNullOrEmpty(userIdString))
            {
                return Ok(false);
            }

            var userId = Guid.Parse(userIdString);
            var isEligible = await orderService.CheckDiscountEligibilityAsync(userId, cancellationToken);
            
            Console.WriteLine($"[CheckDiscount] User: {userId}, Eligible: {isEligible}");
            return Ok(isEligible);
        }

        [HttpGet("my")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMyOrders([FromQuery] OrderSpecificationParameters parameters, [FromQuery] Guid? guestToken, CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                               ?? User.FindFirstValue("sub");
            
            Guid? userId = string.IsNullOrEmpty(userIdString) ? null : Guid.Parse(userIdString);

            if (userId == null && guestToken == null)
            {
                return Ok(new PagedList<OrderSummaryDto>());
            }

            var pagedOrders = await orderService.GetMyOrdersAsync(userId, guestToken, parameters, cancellationToken);
            return Ok(pagedOrders);
        }


        [HttpPost("{id:guid}/pay")]
        [AllowAnonymous]
        public async Task<IActionResult> Pay(Guid id, [FromQuery] Guid? guestToken, CancellationToken cancellationToken)
        {
            var paymentUrl = await orderService.GeneratePaymentUrlAsync(id, guestToken, cancellationToken);

            return Ok(new { PaymentUrl = paymentUrl });
        }




        [HttpPost("liqpay-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> LiqPayCallback([FromForm] string data, [FromForm] string signature, CancellationToken cancellationToken)
        {
            await orderService.ProcessPaymentCallbackAsync(data, signature, cancellationToken);
            return Ok();
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] OrderUpdateDto dto, CancellationToken cancellationToken)
        {
            var updatedOrder = await orderService.UpdateStatusAsync(id, dto, cancellationToken);
            return Ok(updatedOrder);
        }

        [HttpDelete("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid? guestToken, CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            Guid? userId = string.IsNullOrEmpty(userIdString) ? null : Guid.Parse(userIdString);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var order = await orderService.GetByIdAsync(id, guestToken, cancellationToken);
            
            if (role != "Admin" && order.UserId != userId && order.GuestToken != guestToken)
                return Forbid();

            await orderService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}