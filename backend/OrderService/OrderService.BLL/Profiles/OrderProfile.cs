using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OrderService.BLL.DTOs.OrderDTOs;
using OrderService.BLL.DTOs.OrderStatusDTOs;
using OrderService.Domain.Entities;

namespace OrderService.BLL.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderItem, OrderItemReadDto>();
            CreateMap<OrderItemCreateDto, OrderItem>();
   
            CreateMap<DeliveryInformation, DeliveryInformationReadDto>();
            CreateMap<DeliveryInformationCreateDto, DeliveryInformation>();

            CreateMap<OrderStatus, OrderStatusReadDto>();
            
            CreateMap<Order, OrderSummaryDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Items.Sum(i => i.Price * i.Count)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<Order, OrderDetailDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.DeliveryInformation, opt => opt.MapFrom(src => src.DeliveryInformation))
                .ForMember(dest => dest.Gifts, opt => opt.MapFrom(src => src.OrderGifts.Select(og => og.Gift)))
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore()); 
        }
    }
}
