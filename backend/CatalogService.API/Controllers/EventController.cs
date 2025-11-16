using CatalogService.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // Отримати всі події
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventService.GetAllAsync();
            return Ok(events);
        }

        // Отримати подію за id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            return Ok(ev);
        }

        // Створити подію
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            var created = await _eventService.CreateAsync(name);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // Оновити подію
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] string name)
        {
            var updated = await _eventService.UpdateAsync(id, name);
            return Ok(updated);
        }

        // Видалити подію
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _eventService.DeleteAsync(id);
            return NoContent();
        }
    }
}
