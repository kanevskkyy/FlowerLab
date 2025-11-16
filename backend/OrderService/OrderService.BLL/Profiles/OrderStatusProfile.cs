using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OrderService.BLL.DTOs.OrderStatusDTOs;
using OrderService.Domain.Entities;

namespace OrderService.BLL.Profiles
{
    public class OrderStatusProfile : Profile
    {
        public OrderStatusProfile()
        {
            CreateMap<OrderStatus, OrderStatusReadDto>();

            CreateMap<OrderStatusCreateDto, OrderStatus>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow.ToUniversalTime()))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow.ToUniversalTime()));

            CreateMap<OrderStatusUpdateDto, OrderStatus>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow.ToUniversalTime()))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
