using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.BLL.DTO;
using CatalogService.BLL.Services.Interfaces;
using Grpc.Core;

namespace CatalogService.BLL.GrpcServer
{
    public class BouquetServiceGrpc : BouquetGrpcService.BouquetGrpcServiceBase
    {
        private IBouquetService _bouquetService;

        public BouquetServiceGrpc(IBouquetService bouquetService)
        {
            _bouquetService = bouquetService;
        }

        public override async Task<BouquetResponse> GetBouquetById(GetBouquetRequest request, ServerCallContext context)
        {
            var bouquet = await _bouquetService.GetByIdAsync(Guid.Parse(request.Id));

            if (bouquet == null) throw new RpcException(new Status(StatusCode.NotFound, "Букет не знайдено."));

            return Map(bouquet);
        }

        private BouquetResponse Map(BouquetDto dto)
        {
            BouquetResponse response = new BouquetResponse
            {
                Id = dto.Id.ToString(),
                Name = dto.Name,
                Description = dto.Description ?? "",
                Price = (double)dto.Price,
                MainPhotoUrl = dto.MainPhotoUrl,
                CreatedAt = dto.CreatedAt.ToString("O")
            };

            if (dto.Size != null)
            {
                response.Size = new SizeModel
                {
                    Id = dto.Size.Id.ToString(),
                    Name = dto.Size.Name
                };
            }

            response.Flowers.AddRange(dto.Flowers.Select(f => new FlowerInBouquetModel
            {
                Id = f.Id.ToString(),
                Name = f.Name,
                Color = f.Color,
                Quantity = f.Quantity
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
