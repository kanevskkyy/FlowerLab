using System;
using System.Linq;
using System.Threading.Tasks;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using Grpc.Core;

namespace CatalogService.BLL.GrpcServer
{
    public class BouquetServiceGrpc : BouquetGrpcService.BouquetGrpcServiceBase
    {
        private IBouquetService bouquetService;

        public BouquetServiceGrpc(IBouquetService bouquetService)
        {
            this.bouquetService = bouquetService;
        }

        public override async Task<BouquetResponse> GetBouquetById(GetBouquetRequest request, ServerCallContext context)
        {
            BouquetDto bouquet = await bouquetService.GetByIdAsync(Guid.Parse(request.Id));
            if (bouquet == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Bouquet not found."));

            return Map(bouquet);
        }

        public override async Task<GetBouquetsResponse> GetBouquets(GetBouquetsRequest request, ServerCallContext context)
        {
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

            response.Items.AddRange(result.Items.Select(b => 
            {
                var model = new BouquetSimpleModel
                {
                    Id = b.Id.ToString(),
                    Name = b.Name,
                    Price = (double)b.Price,
                    MainPhotoUrl = b.MainPhotoUrl
                };

                model.Sizes.AddRange(b.Sizes.Select(s => new BouquetSizeModel
                {
                    SizeId = s.SizeId.ToString(),
                    SizeName = s.SizeName,
                    Price = (double)s.Price,
                    MaxAssemblableCount = s.MaxAssemblableCount,
                    IsAvailable = s.IsAvailable,
                    Flowers =
                    {
                        s.Flowers.Select(f => new FlowerInBouquetModel
                        {
                            Id = f.Id.ToString(),
                            Name = f.Name,
                            Quantity = f.Quantity
                        })
                    },
                    // Images - skipping for simple list if not needed, but Proto has it required? No, repeated.
                    // If frontend catalog grid needs images on hover, we might need them.
                    // Assuming list view mainly uses MainPhotoUrl.
                }));
                
                return model;
            }));

            return response;
        }


        private BouquetResponse Map(BouquetDto dto)
        {
            BouquetResponse response = new BouquetResponse
            {
                Id = dto.Id.ToString(),
                Name = dto.Name,
                Description = dto.Description ?? "",
                MainPhotoUrl = dto.MainPhotoUrl,
                CreatedAt = dto.CreatedAt.ToString("O")
            };

            response.Sizes.AddRange(dto.Sizes.Select(s => new BouquetSizeModel
            {
                SizeId = s.SizeId.ToString(),
                SizeName = s.SizeName,
                Price = (double)s.Price,
                MaxAssemblableCount = s.MaxAssemblableCount,
                IsAvailable = s.IsAvailable,
                Flowers =
                {
                    s.Flowers.Select(f => new FlowerInBouquetModel
                    {
                        Id = f.Id.ToString(),
                        Name = f.Name,
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
                Name = e.Name
            }));

            response.Recipients.AddRange(dto.Recipients.Select(r => new RecipientModel
            {
                Id = r.Id.ToString(),
                Name = r.Name
            }));

            return response;
        }
    }
}