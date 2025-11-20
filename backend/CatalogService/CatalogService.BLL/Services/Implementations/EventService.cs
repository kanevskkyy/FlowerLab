using AutoMapper;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Exceptions;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public EventService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EventDto>> GetAllAsync()
        {
            var events = await _uow.Events.GetAllAsync();
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<EventDto> GetByIdAsync(Guid id)
        {
            var ev = await _uow.Events.GetByIdAsync(id);
            if (ev == null) throw new NotFoundException($"Подія {id} не знайдена");
            return _mapper.Map<EventDto>(ev);
        }

        public async Task<EventDto> CreateAsync(string name)
        {
            if (await _uow.Events.ExistsWithNameAsync(name))
                throw new AlreadyExistsException($"Подія '{name}' уже існує.");

            var entity = new Event { Name = name };
            await _uow.Events.AddAsync(entity);
            await _uow.SaveChangesAsync();

            return _mapper.Map<EventDto>(entity);
        }

        public async Task<EventDto> UpdateAsync(Guid id, string name)
        {
            var ev = await _uow.Events.GetByIdAsync(id);
            if (ev == null) throw new NotFoundException($"Подія {id} не знайдена");

            if (await _uow.Events.ExistsWithNameAsync(name, id))
                throw new AlreadyExistsException($"Подія '{name}' уже існує.");

            ev.Name = name;
            _uow.Events.Update(ev);
            await _uow.SaveChangesAsync();

            return _mapper.Map<EventDto>(ev);
        }

        public async Task DeleteAsync(Guid id)
        {
            var ev = await _uow.Events.GetByIdAsync(id);
            if (ev == null) throw new NotFoundException($"Подія {id} не знайдена");

            _uow.Events.Delete(ev);
            await _uow.SaveChangesAsync();
        }
    }

}
