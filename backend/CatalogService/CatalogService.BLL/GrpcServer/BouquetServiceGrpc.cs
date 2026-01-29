using System;
using System.Linq;
using System.Threading.Tasks;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using Grpc.Core;
using shared.localization;

namespace CatalogService.BLL.GrpcServer
{
    public class BouquetServiceGrpc : BouquetGrpcService.BouquetGrpcServiceBase
    {
        private IBouquetService bouquetService;
        private ILanguageProvider languageProvider;

        public BouquetServiceGrpc(IBouquetService bouquetService, ILanguageProvider languageProvider)
        {
            this.bouquetService = bouquetService;
            this.languageProvider = languageProvider;
        }

        private void SetCurrentLanguage(ServerCallContext context)
        {
            var acceptLang = context.RequestHeaders.FirstOrDefault(h => h.Key == "accept-language")?.Value;
            if (!string.IsNullOrEmpty(acceptLang))
            {
                var lang = acceptLang.Split(',')[0].Split(';')[0].Split('-')[0].ToLower();
                if (lang == "uk" || lang == "ua" || lang == "ukr") lang = "ua";
                else if (lang == "en" || lang == "eng") lang = "en";
                else lang = null;

                if (lang != null) languageProvider.CurrentLanguage = lang;
            }
        }

        public override async Task<BouquetResponse> GetBouquetById(GetBouquetRequest request, ServerCallContext context)
        {
            SetCurrentLanguage(context);
            BouquetDto bouquet = await bouquetService.GetByIdAsync(Guid.Parse(request.Id));
            if (bouquet == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Bouquet not found."));

            return Map(bouquet);
        }

        public override async Task<GetBouquetsResponse> GetBouquets(GetBouquetsRequest request, ServerCallContext context)
        {
            SetCurrentLanguage(context);
            var query = new CatalogService.Domain.QueryParametrs.BouquetQueryParameters
            {
                Page = request.Page > 0 ? request.Page : 1,
                PageSize = request.PageSize > 0 ? request.PageSize : 9,
                SortBy = request.SortBy,
                Name = request.Name,
                MinPrice = request.MinPrice > 0 ? (decimal?)request.MinPrice : null,
                MaxPrice = request.MaxPrice > 0 ? (decimal?)request.MaxPrice : null,
                FlowerIds = request.FlowerIds?.Select(Guid.Parse).ToList(),
                EventIds = request.EventIds?.Select(Guid.Parse).ToList(),
                RecipientIds = request.RecipientIds?.Select(Guid.Parse).ToList(),
                SizeIds = request.SizeIds?.Select(Guid.Parse).ToList(),
                Quantities = request.Quantities?.ToList()
            };

            var result = await bouquetService.GetAllAsync(query);

            var response = new GetBouquetsResponse
            {
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage
            };

            var currentLang = languageProvider.CurrentLanguage ?? "ua";

            response.Items.AddRange(result.Items.Select(b => 
            {
                var model = new BouquetSimpleModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name.GetValueOrDefault(currentLang, b.Name.Values.FirstOrDefault() ?? ""),
                    Price = (double)b.Price,
                    MainPhotoUrl = b.MainPhotoUrl
                };

                model.Sizes.AddRange(b.Sizes.Select(s => new BouquetSizeModel
                {
                    SizeId = s.SizeId.ToString(),
                    SizeName = s.SizeName.GetValueOrDefault(currentLang, s.SizeName.Values.FirstOrDefault() ?? ""),
                    Price = (double)s.Price,
                    MaxAssemblableCount = s.MaxAssemblableCount,
                    IsAvailable = s.IsAvailable,
                    Flowers =
                    {
                        s.Flowers.Select(f => new FlowerInBouquetModel
                        {
                            Id = f.Id.ToString(),
                            Name = f.Name.GetValueOrDefault(currentLang, f.Name.Values.FirstOrDefault() ?? ""),
                            Quantity = f.Quantity
                        })
                    },
                }));
                
                return model;
            }));

            return response;
        }


        private BouquetResponse Map(BouquetDto dto)
        {
            var currentLang = languageProvider.CurrentLanguage ?? "ua";
            BouquetResponse response = new BouquetResponse
            {
                Id = dto.Id.ToString(),
                Name = dto.Name.GetValueOrDefault(currentLang, dto.Name.Values.FirstOrDefault() ?? ""),
                Description = dto.Description.GetValueOrDefault(currentLang, dto.Description?.Values.FirstOrDefault() ?? ""),
                MainPhotoUrl = dto.MainPhotoUrl,
                CreatedAt = dto.CreatedAt.ToString("O")
            };

            response.Sizes.AddRange(dto.Sizes.Select(s => new BouquetSizeModel
            {
                SizeId = s.SizeId.ToString(),
                SizeName = s.SizeName.GetValueOrDefault(currentLang, s.SizeName.Values.FirstOrDefault() ?? ""),
                Price = (double)s.Price,
                MaxAssemblableCount = s.MaxAssemblableCount,
                IsAvailable = s.IsAvailable,
                Flowers =
                {
                    s.Flowers.Select(f => new FlowerInBouquetModel
                    {
                        Id = f.Id.ToString(),
                        Name = f.Name.GetValueOrDefault(currentLang, f.Name.Values.FirstOrDefault() ?? ""),
                        Quantity = f.Quantity
                    })
                },
                Images =
                {
                    s.Images.Select(i => new BouquetImageModel
                    {
                        Id = i.Id.ToString(),
                        ImageUrl = i.ImageUrl,
                        Position = i.Position,
                        IsMain = i.IsMain
                    })
                }
            }));

            response.Events.AddRange(dto.Events.Select(e => new EventModel
            {
                Id = e.Id.ToString(),
                Name = e.Name.GetValueOrDefault(currentLang, e.Name.Values.FirstOrDefault() ?? "")
            }));

            response.Recipients.AddRange(dto.Recipients.Select(r => new RecipientModel
            {
                Id = r.Id.ToString(),
                Name = r.Name.GetValueOrDefault(currentLang, r.Name.Values.FirstOrDefault() ?? "")
            }));

            return response;
        }
    }
}