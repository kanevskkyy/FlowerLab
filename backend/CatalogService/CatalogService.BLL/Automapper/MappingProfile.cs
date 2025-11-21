using AutoMapper;
using CatalogService.BLL.DTO;
using CatalogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.BLL.Automapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Bouquet, BouquetDto>()
            .ForMember(d => d.Flowers, opt => opt.MapFrom(s => s.BouquetFlowers))
            .ForMember(d => d.Size, opt => opt.MapFrom(s => s.BouquetSizes.Select(bs => bs.Size).FirstOrDefault()))
            .ForMember(d => d.Events, opt => opt.MapFrom(s => s.BouquetEvents.Select(be => be.Event)))
            .ForMember(d => d.Recipients, opt => opt.MapFrom(s => s.BouquetRecipients.Select(br => br.Recipient)))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.BouquetImages))
            .ForMember(d => d.IsAvailable, opt => opt.MapFrom(s =>
                s.BouquetFlowers.All(bf => bf.Flower.Quantity >= bf.Quantity)
            ))
            .ForMember(d => d.MaxAssemblableCount, opt => opt.MapFrom(s =>
                s.BouquetFlowers.Any()
                    ? s.BouquetFlowers.Min(bf => bf.Flower.Quantity / bf.Quantity)
                    : 0
            ));

            CreateMap<BouquetFlower, FlowerInBouquetDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Flower.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Flower.Name))
                .ForMember(d => d.Color, opt => opt.MapFrom(s => s.Flower.Color))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.Quantity));

            CreateMap<Size, SizeDto>();
            CreateMap<Event, EventDto>();
            CreateMap<Recipient, RecipientDto>();
            CreateMap<BouquetImage, BouquetImageDto>();
            CreateMap<Flower, FlowerDto>();
            CreateMap<Flower, FlowerCreateUpdateDto>();
        }
    }
}
