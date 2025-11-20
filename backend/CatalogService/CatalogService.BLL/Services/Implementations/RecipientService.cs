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
    public class RecipientService : IRecipientService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public RecipientService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecipientDto>> GetAllAsync()
        {
            var recs = await _uow.Recipients.GetAllAsync();
            return _mapper.Map<IEnumerable<RecipientDto>>(recs);
        }

        public async Task<RecipientDto> GetByIdAsync(Guid id)
        {
            var rec = await _uow.Recipients.GetByIdAsync(id);
            if (rec == null) throw new NotFoundException($"Отримувач з ID {id} не знайдений");
            return _mapper.Map<RecipientDto>(rec);
        }

        public async Task<RecipientDto> CreateAsync(string name)
        {
            if (await _uow.Recipients.ExistsWithNameAsync(name))
                throw new AlreadyExistsException($"Отримувач '{name}' уже існує.");

            var entity = new Recipient { Name = name };
            await _uow.Recipients.AddAsync(entity);
            await _uow.SaveChangesAsync();

            return _mapper.Map<RecipientDto>(entity);
        }

        public async Task<RecipientDto> UpdateAsync(Guid id, string name)
        {
            var rec = await _uow.Recipients.GetByIdAsync(id);
            if (rec == null) throw new NotFoundException($"Отримувач з ID {id} не знайдений");

            if (await _uow.Recipients.ExistsWithNameAsync(name, id))
                throw new AlreadyExistsException($"Отримувач '{name}' уже існує.");

            rec.Name = name;
            _uow.Recipients.Update(rec);
            await _uow.SaveChangesAsync();

            return _mapper.Map<RecipientDto>(rec);
        }

        public async Task DeleteAsync(Guid id)
        {
            var rec = await _uow.Recipients.GetByIdAsync(id);
            if (rec == null) throw new NotFoundException($"Отримувач з ID {id} не знайдений");

            _uow.Recipients.Delete(rec);
            await _uow.SaveChangesAsync();
        }
    }

}
