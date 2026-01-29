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
            CreateMap<OrderItem, OrderItemReadDto>()
                .ForMember(dest => dest.BouquetName, opt => opt.MapFrom(src => new Dictionary<string, string> { { "en", src.BouquetName }, { "ua", src.BouquetName } }))
                .ForMember(dest => dest.SizeName, opt => opt.MapFrom(src => new Dictionary<string, string> { { "en", src.SizeName }, { "ua", src.SizeName } }));
            CreateMap<OrderItemCreateDto, OrderItem>();
   
            CreateMap<DeliveryInformation, DeliveryInformationReadDto>();
            CreateMap<DeliveryInformationCreateDto, DeliveryInformation>();


            CreateMap<OrderGift, OrderGiftReadDto>()
                .ForMember(dest => dest.Gift, opt => opt.MapFrom(src => src.Gift))
                .ForMember(dest => dest.OrderedCount, opt => opt.MapFrom(src => src.Count));

            CreateMap<Order, OrderSummaryDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.OrderGifts, opt => opt.MapFrom(src => src.OrderGifts)) // Map gifts
                .ForMember(dest => dest.GiftMessage, opt => opt.MapFrom(src => src.GiftMessage));

            CreateMap<Order, OrderDetailDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.DeliveryInformation, opt => opt.MapFrom(src => src.DeliveryInformation))
                .ForMember(dest => dest.Gifts, opt => opt.MapFrom(src => src.OrderGifts)) 
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));
        }
    }
}
