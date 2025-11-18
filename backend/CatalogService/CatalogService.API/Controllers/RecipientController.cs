using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers
{
    [ApiController]
    [Route("api/recipients")]
    public class RecipientsController : ControllerBase
    {
        private readonly IRecipientService _recipientService;

        public RecipientsController(IRecipientService recipientService)
        {
            _recipientService = recipientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _recipientService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id) =>
            Ok(await _recipientService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RecipientCreateDto dto)
        {
            var created = await _recipientService.CreateAsync(dto.Name);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RecipientUpdateDto dto)
        {
            var updated = await _recipientService.UpdateAsync(id, dto.Name);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _recipientService.DeleteAsync(id);
            return NoContent();
        }
    }
}
