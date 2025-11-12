using Microsoft.AspNetCore.Mvc;
using OrderService.BLL.DTOs.GiftsDTOs;
using OrderService.BLL.Services.Interfaces;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;

        public GiftController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var gifts = await _giftService.GetAllAsync();
            return Ok(gifts);
        }

        [HttpGet("{id:guid}", Name = "GetGiftById")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var gift = await _giftService.GetByIdAsync(id);
            return Ok(gift);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] GiftCreateDto dto)
        {
            var createdGift = await _giftService.CreateAsync(dto);
            return CreatedAtRoute("GetGiftById", new { id = createdGift.Id }, createdGift);
        }


        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromForm] GiftUpdateDto dto)
        {
            var updatedGift = await _giftService.UpdateAsync(id, dto);
            return Ok(updatedGift);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _giftService.DeleteAsync(id);
            return NoContent();
        }
    }
}