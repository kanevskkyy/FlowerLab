using Microsoft.AspNetCore.Mvc;
using OrderService.BLL.DTOs.GiftsDTOs;
using OrderService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/gifts")]
    public class GiftController : ControllerBase
    {
        private IGiftService giftService;

        public GiftController(IGiftService giftService)
        {
            this.giftService = giftService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var gifts = await giftService.GetAllAsync();
            return Ok(gifts);
        }

        [HttpGet("{id:guid}", Name = "GetGiftById")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var gift = await giftService.GetByIdAsync(id);
            return Ok(gift);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync([FromForm] GiftCreateDto dto)
        {
            var createdGift = await giftService.CreateAsync(dto);
            return CreatedAtRoute("GetGiftById", new { id = createdGift.Id }, createdGift);
        }


        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromForm] GiftUpdateDto dto)
        {
            var updatedGift = await giftService.UpdateAsync(id, dto);
            return Ok(updatedGift);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await giftService.DeleteAsync(id);
            return NoContent();
        }
    }
}