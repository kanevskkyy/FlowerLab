using CatalogService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _sizeService;

        public SizesController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _sizeService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await _sizeService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            var created = await _sizeService.CreateAsync(name);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] string name)
        {
            var updated = await _sizeService.UpdateAsync(id, name);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sizeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
