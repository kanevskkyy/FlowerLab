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
                throw new RpcException(new Status(StatusCode.NotFound, "Букет не знайдено."));

            return Map(bouquet);
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
                Flowers = { s.Flowers.Select(f => new FlowerInBouquetModel
                {
                    Id = f.Id.ToString(),
                    Name = f.Name,
                    Color = f.Color,
                    Quantity = f.Quantity
                })}
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

            response.Images.AddRange(dto.Images.Select(i => new BouquetImageModel
            {
                Id = i.Id.ToString(),
                ImageUrl = i.ImageUrl,
                Position = i.Position
            }));

            return response;
        }
    }
}