using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OrderService.BLL.DTOs.GiftsDTOs;
using OrderService.Domain.Entities;

namespace OrderService.BLL.Profiles
{
    public class GiftProfile : Profile
    {
        public GiftProfile()
        {
            CreateMap<Gift, GiftReadDto>();

            CreateMap<GiftCreateDto, Gift>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow.ToUniversalTime()));

            CreateMap<GiftUpdateDto, Gift>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow.ToUniversalTime()));
        }
    }
}
