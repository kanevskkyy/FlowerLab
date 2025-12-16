using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace CatalogService.API.Controllers
{

    [ApiController]
    [Route("api/flowers")]
    public class FlowersController : ControllerBase
    {
        private IFlowerService flowerService;

        public FlowersController(IFlowerService flowerService)
        {
            this.flowerService = flowerService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var flowers = await flowerService.GetAllAsync();
            return Ok(flowers);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var flower = await flowerService.GetByIdAsync(id);
            return Ok(flower);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] FlowerCreateUpdateDto dto)
        {
            var created = await flowerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FlowerCreateUpdateDto dto)
        {
            var updated = await flowerService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await flowerService.DeleteAsync(id);
            return NoContent();
        }
    }
}
