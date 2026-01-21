using AutoMapper;
using CatalogService.BLL.DTO;
using CatalogService.Domain.Entities;
using System.Linq;

namespace CatalogService.BLL.Automapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Bouquet, BouquetDto>()
                .ForMember(d => d.Sizes, opt => opt.MapFrom(s => s.BouquetSizes))
                .ForMember(d => d.Events, opt => opt.MapFrom(s => s.BouquetEvents.Select(be => be.Event)))
                .ForMember(d => d.Recipients, opt => opt.MapFrom(s => s.BouquetRecipients.Select(br => br.Recipient)));

            CreateMap<BouquetSize, BouquetSizeDto>()
                .ForMember(d => d.SizeId, opt => opt.MapFrom(s => s.SizeId))
                .ForMember(d => d.SizeName, opt => opt.MapFrom(s => s.Size.Name))
                .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Price))
                .ForMember(d => d.Flowers, opt => opt.MapFrom(s => s.BouquetSizeFlowers))
                .ForMember(d => d.Images, opt => opt.MapFrom(s => s.BouquetImages.OrderBy(img => img.Position)))
                .ForMember(d => d.MaxAssemblableCount, opt => opt.MapFrom(s =>
                    s.BouquetSizeFlowers.Any()
                        ? s.BouquetSizeFlowers.Min(bsf => bsf.Flower.Quantity / bsf.Quantity)
                        : 0))
                .ForMember(d => d.IsAvailable, opt => opt.MapFrom(s =>
                    s.BouquetSizeFlowers.All(bsf => bsf.Flower.Quantity >= bsf.Quantity)));

            CreateMap<BouquetSizeFlower, FlowerInBouquetDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.FlowerId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Flower.Name))
                .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.Quantity));

            CreateMap<Bouquet, BouquetSummaryDto>()
                .ForMember(d => d.Sizes, opt => opt.MapFrom(s => s.BouquetSizes))
                .ForMember(d => d.Price, opt => opt.MapFrom(s =>
                    s.BouquetSizes.Any() ? s.BouquetSizes.Min(bs => bs.Price) : 0m))
                .ForMember(d => d.MainPhotoUrl, opt => opt.MapFrom(s => s.MainPhotoUrl));

            CreateMap<BouquetImage, BouquetImageDto>();
            CreateMap<Size, SizeDto>();
            CreateMap<Event, EventDto>();
            CreateMap<Recipient, RecipientDto>();
            CreateMap<Flower, FlowerDto>();
        }
    }
}