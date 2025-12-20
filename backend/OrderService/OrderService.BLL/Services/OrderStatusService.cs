using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OrderService.BLL.DTOs.OrderStatusDTOs;
using OrderService.BLL.Exceptions;
using OrderService.BLL.Services.Interfaces;
using OrderService.DAL.UOW;
using OrderService.Domain.Entities;

namespace OrderService.BLL.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;

        public OrderStatusService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<OrderStatusReadDto>> GetAllAsync()
        {
            var statuses = await unitOfWork.OrderStatuses.GetAllAsync();
            return mapper.Map<IEnumerable<OrderStatusReadDto>>(statuses);
        }

        public async Task<OrderStatusReadDto> GetByIdAsync(Guid id)
        {
            var status = await unitOfWork.OrderStatuses.GetByIdAsync(id);
            if (status == null)
                throw new NotFoundException($"Order status with ID {id} was not found");

            return mapper.Map<OrderStatusReadDto>(status);
        }

        public async Task<OrderStatusReadDto> CreateAsync(OrderStatusCreateDto dto)
        {
            var isDuplicate = await unitOfWork.OrderStatuses.IsNameDuplicatedAsync(dto.Name);
            if (isDuplicate)
                throw new AlreadyExistsException($"Order status '{dto.Name}' already exists");

            var entity = mapper.Map<OrderStatus>(dto);

            await unitOfWork.OrderStatuses.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<OrderStatusReadDto>(entity);
        }

        public async Task<OrderStatusReadDto> UpdateAsync(Guid id, OrderStatusUpdateDto dto)
        {
            var status = await unitOfWork.OrderStatuses.GetByIdAsync(id);
            if (status == null)
                throw new NotFoundException($"Order status with ID {id} was not found");

            var isDuplicate = await unitOfWork.OrderStatuses.IsNameDuplicatedAsync(dto.Name, id);
            if (isDuplicate)
                throw new AlreadyExistsException($"Order status '{dto.Name}' already exists");

            mapper.Map(dto, status);

            unitOfWork.OrderStatuses.Update(status);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<OrderStatusReadDto>(status);
        }

        public async Task DeleteAsync(Guid id)
        {
            var status = await unitOfWork.OrderStatuses.GetByIdAsync(id);
            if (status == null)
                throw new NotFoundException($"Order status with ID {id} was not found");

            unitOfWork.OrderStatuses.Delete(status);
            await unitOfWork.SaveChangesAsync();
        }

    }
}
