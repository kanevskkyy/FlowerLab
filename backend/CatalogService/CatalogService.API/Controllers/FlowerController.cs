using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers
{

    [ApiController]
    [Route("api/flowers")]
    public class FlowersController : ControllerBase
    {
        private readonly IFlowerService _flowerService;

        public FlowersController(IFlowerService flowerService)
        {
            _flowerService = flowerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var flowers = await _flowerService.GetAllAsync();
            return Ok(flowers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var flower = await _flowerService.GetByIdAsync(id);
            return Ok(flower);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FlowerCreateUpdateDto dto)
        {
            var created = await _flowerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FlowerCreateUpdateDto dto)
        {
            var updated = await _flowerService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _flowerService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(Guid id, [FromQuery] int quantity)
        {
            var updated = await _flowerService.UpdateStockAsync(id, quantity);
            return Ok(updated);
        }
    }
}
