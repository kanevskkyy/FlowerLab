using Microsoft.AspNetCore.Mvc;
using OrderService.BLL.DTOs.GiftsDTOs;
using OrderService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Threading;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/gifts")]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService giftService;

        public GiftController(IGiftService giftService)
        {
            this.giftService = giftService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var gifts = await giftService.GetAllAsync(cancellationToken);
            return Ok(gifts);
        }

        [HttpGet("{id:guid}", Name = "GetGiftById")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var gift = await giftService.GetByIdAsync(id, cancellationToken);
            return Ok(gift);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(
            [FromForm] GiftCreateDto dto,
            CancellationToken cancellationToken)
        {
            if (Request.Form.Keys.Any())
            {
                foreach (var key in Request.Form.Keys)
                {
                    if (key.StartsWith("Name[") && key.EndsWith("]"))
                    {
                        var lang = key.Substring(5, key.Length - 6);
                        dto.Name[lang] = Request.Form[key]!;
                    }
                    if (key.StartsWith("Description[") && key.EndsWith("]"))
                    {
                        var lang = key.Substring(12, key.Length - 13);
                        if (dto.Description == null) dto.Description = new Dictionary<string, string>();
                        dto.Description[lang] = Request.Form[key]!;
                    }
                    if (key == "Price")
                    {
                        if (decimal.TryParse(Request.Form[key], out var price)) dto.Price = price;
                    }
                    if (key == "AvailableCount")
                    {
                        if (int.TryParse(Request.Form[key], out var count)) dto.AvailableCount = count;
                    }
                }
            }

            var createdGift = await giftService.CreateAsync(dto, cancellationToken);
            return CreatedAtRoute("GetGiftById", new { id = createdGift.Id }, createdGift);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(
            Guid id,
            [FromForm] GiftUpdateDto dto,
            CancellationToken cancellationToken)
        {
            if (Request.Form.Keys.Any())
            {
                foreach (var key in Request.Form.Keys)
                {
                    if (key.StartsWith("Name[") && key.EndsWith("]"))
                    {
                        var lang = key.Substring(5, key.Length - 6);
                        dto.Name[lang] = Request.Form[key]!;
                    }
                    if (key.StartsWith("Description[") && key.EndsWith("]"))
                    {
                        var lang = key.Substring(12, key.Length - 13);
                        if (dto.Description == null) dto.Description = new Dictionary<string, string>();
                        dto.Description[lang] = Request.Form[key]!;
                    }
                    if (key == "Price")
                    {
                        if (decimal.TryParse(Request.Form[key], out var price)) dto.Price = price;
                    }
                    if (key == "AvailableCount")
                    {
                        if (int.TryParse(Request.Form[key], out var count)) dto.AvailableCount = count;
                    }
                }
            }

            var updatedGift = await giftService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updatedGift);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await giftService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}