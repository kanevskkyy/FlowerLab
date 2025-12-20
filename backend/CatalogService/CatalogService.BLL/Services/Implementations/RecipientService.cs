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

        public async Task<IEnumerable<RecipientDto>> GetAllAsync()
        {
            IEnumerable<Recipient> recipients = await uow.Recipients.GetAllAsync();
            return mapper.Map<IEnumerable<RecipientDto>>(recipients);
        }

        public async Task<RecipientDto> GetByIdAsync(Guid id)
        {
            Recipient? recipient = await uow.Recipients.GetByIdAsync(id);
            if (recipient == null)
                throw new NotFoundException($"Recipient with ID {id} not found.");

            return mapper.Map<RecipientDto>(recipient);
        }

        public async Task<RecipientDto> CreateAsync(string name)
        {
            if (await uow.Recipients.ExistsWithNameAsync(name))
                throw new AlreadyExistsException($"Recipient '{name}' already exists.");

            Recipient entity = new Recipient { Name = name };
            await uow.Recipients.AddAsync(entity);
            await uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<RecipientDto>(entity);
        }

        public async Task<RecipientDto> UpdateAsync(Guid id, string name)
        {
            Recipient? recipient = await uow.Recipients.GetByIdAsync(id);
            if (recipient == null)
                throw new NotFoundException($"Recipient with ID {id} not found.");

            if (await uow.Recipients.ExistsWithNameAsync(name, id))
                throw new AlreadyExistsException($"Recipient '{name}' already exists.");

            recipient.Name = name;
            uow.Recipients.Update(recipient);
            await uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();

            return mapper.Map<RecipientDto>(recipient);
        }

        public async Task DeleteAsync(Guid id)
        {
            Recipient? recipient = await uow.Recipients.GetByIdAsync(id);
            if (recipient == null)
                throw new NotFoundException($"Recipient with ID {id} not found.");

            uow.Recipients.Delete(recipient);
            await uow.SaveChangesAsync();

            await entityCacheInvalidationService.InvalidateAllAsync();
        }

    }

}
