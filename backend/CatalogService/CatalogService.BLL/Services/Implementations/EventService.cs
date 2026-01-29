using AutoMapper;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Exceptions;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.DAL.UnitOfWork;
using CatalogService.Domain.Entities;
using shared.cache;
using System;
using System.Collections.Generic;
using System.Threading;
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

        public async Task<IEnumerable<EventDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Event> events = await uow.Events.GetAllAsync(cancellationToken);
            return mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<EventDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Event? targetEvent = await uow.Events.GetByIdAsync(id, cancellationToken);
            if (targetEvent == null) throw new NotFoundException($"Event {id} not found");

            return mapper.Map<EventDto>(targetEvent);
        }

        public async Task<EventDto> CreateAsync(Dictionary<string, string> name, CancellationToken cancellationToken = default)
        {
            if (await uow.Events.ExistsWithNameAsync(name.GetValueOrDefault("ua", ""), cancellationToken: cancellationToken))
                throw new AlreadyExistsException($"Event '{name.GetValueOrDefault("ua", "")}' already exists.");

            Event entity = new Event { Name = name };
            await uow.Events.AddAsync(entity, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<EventDto>(entity);
        }

        public async Task<EventDto> UpdateAsync(Guid id, Dictionary<string, string> name, CancellationToken cancellationToken = default)
        {
            Event? ev = await uow.Events.GetByIdAsync(id, cancellationToken);
            if (ev == null) throw new NotFoundException($"Event {id} not found");

            if (await uow.Events.ExistsWithNameAsync(name.GetValueOrDefault("ua", ""), id, cancellationToken))
                throw new AlreadyExistsException($"Event '{name.GetValueOrDefault("ua", "")}' already exists.");

            ev.Name = name;
            uow.Events.Update(ev);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<EventDto>(ev);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Event? ev = await uow.Events.GetByIdAsync(id, cancellationToken);
            if (ev == null) throw new NotFoundException($"Event {id} not found");

            uow.Events.Delete(ev);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();
        }
    }
}
