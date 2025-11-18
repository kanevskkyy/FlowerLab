using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventService.GetAllAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            return Ok(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventCreateDto dto)
        {
            var created = await _eventService.CreateAsync(dto.Name);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EventUpdateDto dto)
        {
            var updated = await _eventService.UpdateAsync(id, dto.Name);

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _eventService.DeleteAsync(id);
            return NoContent();
        }
    }
}
