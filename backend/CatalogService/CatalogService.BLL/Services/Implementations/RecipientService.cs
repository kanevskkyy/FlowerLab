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
    public class RecipientService : IRecipientService
    {
        private IUnitOfWork uow;
        private IMapper mapper;
        private IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService;

        public RecipientService(IUnitOfWork uow, IMapper mapper, IEntityCacheInvalidationService<FilterResponse> entityCacheInvalidationService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.entityCacheInvalidationService = entityCacheInvalidationService;
        }

        public async Task<IEnumerable<RecipientDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Recipient> recipients = await uow.Recipients.GetAllAsync(cancellationToken);
            return mapper.Map<IEnumerable<RecipientDto>>(recipients);
        }

        public async Task<RecipientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Recipient? recipient = await uow.Recipients.GetByIdAsync(id, cancellationToken);
            if (recipient == null)
                throw new NotFoundException($"Recipient with ID {id} not found.");

            return mapper.Map<RecipientDto>(recipient);
        }

        public async Task<RecipientDto> CreateAsync(Dictionary<string, string> name, CancellationToken cancellationToken = default)
        {
            if (await uow.Recipients.ExistsWithNameAsync(name.GetValueOrDefault("ua", ""), cancellationToken: cancellationToken))
                throw new AlreadyExistsException($"Recipient '{name.GetValueOrDefault("ua", "")}' already exists.");

            Recipient entity = new Recipient { Name = name };
            await uow.Recipients.AddAsync(entity, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<RecipientDto>(entity);
        }

        public async Task<RecipientDto> UpdateAsync(Guid id, Dictionary<string, string> name, CancellationToken cancellationToken = default)
        {
            Recipient? recipient = await uow.Recipients.GetByIdAsync(id, cancellationToken);
            if (recipient == null)
                throw new NotFoundException($"Recipient with ID {id} not found.");

            if (await uow.Recipients.ExistsWithNameAsync(name.GetValueOrDefault("ua", ""), id, cancellationToken))
                throw new AlreadyExistsException($"Recipient '{name.GetValueOrDefault("ua", "")}' already exists.");

            recipient.Name = name;
            uow.Recipients.Update(recipient);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<RecipientDto>(recipient);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Recipient? recipient = await uow.Recipients.GetByIdAsync(id, cancellationToken);
            if (recipient == null)
                throw new NotFoundException($"Recipient with ID {id} not found.");

            uow.Recipients.Delete(recipient);
            await uow.SaveChangesAsync(cancellationToken);

            await entityCacheInvalidationService.InvalidateAllAsync();
        }
    }
}
