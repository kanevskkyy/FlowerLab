using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var flowers = await flowerService.GetAllAsync(cancellationToken);
            return Ok(flowers);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var flower = await flowerService.GetByIdAsync(id, cancellationToken);
            return Ok(flower);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] FlowerCreateUpdateDto dto, CancellationToken cancellationToken)
        {
            var created = await flowerService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FlowerCreateUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await flowerService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await flowerService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
