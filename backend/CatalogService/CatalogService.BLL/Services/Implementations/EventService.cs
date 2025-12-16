using AutoMapper;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Exceptions;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using shared.cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services.Implementations
{
    public class EventService : IEventService
    {
        private IUnitOfWork uow;
        private IMapper mapper;
        private IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService;

        public EventService(IUnitOfWork uow, IMapper mapper, IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService)
        {
            this.uow = uow;
            this.entityCacheInvalidationService = entityCacheInvalidationService;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<EventDto>> GetAllAsync()
        {
            IEnumerable<Event> events = await uow.Events.GetAllAsync();
            return mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<EventDto> GetByIdAsync(Guid id)
        {
            Event? targetEvent = await uow.Events.GetByIdAsync(id);
            if (targetEvent == null) throw new NotFoundException($"Подія {id} не знайдена");
            
            return mapper.Map<EventDto>(targetEvent);
        }

        public async Task<EventDto> CreateAsync(string name)
        {
            if (await uow.Events.ExistsWithNameAsync(name))
                throw new AlreadyExistsException($"Подія '{name}' уже існує.");

            Event entity = new Event 
            { 
                Name = name 
            };
            await uow.Events.AddAsync(entity);
            await uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<EventDto>(entity);
        }

        public async Task<EventDto> UpdateAsync(Guid id, string name)
        {
            Event ev = await uow.Events.GetByIdAsync(id);
            if (ev == null) throw new NotFoundException($"Подія {id} не знайдена");

            if (await uow.Events.ExistsWithNameAsync(name, id))
                throw new AlreadyExistsException($"Подія '{name}' уже існує.");

            ev.Name = name;
            uow.Events.Update(ev);
            await uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<EventDto>(ev);
        }

        public async Task DeleteAsync(Guid id)
        {
            Event? ev = await uow.Events.GetByIdAsync(id);
            if (ev == null) throw new NotFoundException($"Подія {id} не знайдена");

            uow.Events.Delete(ev);
            await uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();
        }
    }

}
