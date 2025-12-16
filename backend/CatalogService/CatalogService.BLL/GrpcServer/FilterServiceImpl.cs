using System;
using System.Linq;
using System.Threading.Tasks;
using CatalogService.DAL.UnitOfWork;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CatalogService.BLL.GrpcServer
{
    public class FilterServiceImpl : FilterService.FilterServiceBase
    {
        private IUnitOfWork unitOfWork;
        private ILogger<FilterServiceImpl> logger;

        public FilterServiceImpl(IUnitOfWork unitOfWork, ILogger<FilterServiceImpl> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public override async Task<FilterResponse> GetAllFilters(FilterEmptyRequest request, ServerCallContext context)
        {
            logger.LogInformation("Fetching all filters");

            try
            {
                var sizes = await unitOfWork.Sizes.GetAllAsync();
                var events = await unitOfWork.Events.GetAllAsync();
                var recipients = await unitOfWork.Recipients.GetAllAsync();
                var flowers = await unitOfWork.Flowers.GetAllAsync();

                var sizeResponse = new SizeResponseList();
                sizeResponse.Sizes.AddRange(sizes.Select(s => new SizeResponse
                {
                    Id = s.Id.ToString(),
                    Name = s.Name
                }));

                var eventResponse = new EventResponseList();
                eventResponse.Events.AddRange(events.Select(e => new EventResponse
                {
                    Id = e.Id.ToString(),
                    Name = e.Name
                }));

                var recipientResponse = new ReceivmentResponseList();
                recipientResponse.Receivments.AddRange(recipients.Select(r => new ReceivmentResponse
                {
                    Id = r.Id.ToString(),
                    Name = r.Name
                }));

                var flowerResponse = new FlowerResponseList();
                flowerResponse.Flowers.AddRange(flowers.Select(f => new FlowerResponse
                {
                    Id = f.Id.ToString(),
                    Name = f.Name,
                    Color = f.Color,
                    Description = f.Description,
                    Quantity = f.Quantity
                }));

                return new FilterResponse
                {
                    SizeResponseList = sizeResponse,
                    EventResponseList = eventResponse,
                    ReceivmentResponseList = recipientResponse,
                    FlowerResponseList = flowerResponse
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch filters");
                throw new RpcException(new Status(StatusCode.Internal, "Внутрішня помилка сервера"));
            }
        }
    }
}