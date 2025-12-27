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
    [Route("api/sizes")]
    public class SizesController : ControllerBase
    {
        private ISizeService sizeService;

        public SizesController(ISizeService sizeService)
        {
            this.sizeService = sizeService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
            Ok(await sizeService.GetAllAsync(cancellationToken));

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) =>
            Ok(await sizeService.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] SizeCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await sizeService.CreateAsync(dto.Name, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SizeUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await sizeService.UpdateAsync(id, dto.Name, cancellationToken);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await sizeService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
