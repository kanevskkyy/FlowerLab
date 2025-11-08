using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiftsController : ControllerBase
    {
        private readonly IGiftService _giftService;

        public GiftsController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _giftService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await _giftService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] GiftCreateDto dto)
        {
            FileContentDto? fileContent = null;

            if (dto.Image != null)
            {
                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);

                fileContent = new FileContentDto
                {
                    Content = ms.ToArray(),
                    FileName = dto.Image.FileName
                };
            }

            var created = await _giftService.CreateAsync(dto.Name, dto.GiftType, fileContent);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] GiftCreateDto dto)
        {
            FileContentDto? fileContent = null;

            if (dto.Image != null)
            {
                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);

                fileContent = new FileContentDto
                {
                    Content = ms.ToArray(),
                    FileName = dto.Image.FileName
                };
            }

            // Оновлюємо дані подарунка
            var updated = await _giftService.UpdateAsync(id, dto.Name, dto.GiftType, fileContent);

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _giftService.DeleteAsync(id);
            return NoContent();
        }
    }
}
