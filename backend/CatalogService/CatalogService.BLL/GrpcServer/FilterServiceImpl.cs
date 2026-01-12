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
                var sizes = await unitOfWork.Sizes.GetAllAsync(context.CancellationToken);
                var events = await unitOfWork.Events.GetAllAsync(context.CancellationToken);
                var recipients = await unitOfWork.Recipients.GetAllAsync(context.CancellationToken);
                var flowers = await unitOfWork.Flowers.GetAllAsync(context.CancellationToken);
                (decimal minPrice, decimal maxPrice) = await unitOfWork.Bouquets.GetMinAndMaxPriceAsync(context.CancellationToken);

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
                    Quantity = f.Quantity
                }));

                var priceRangeResponse = new PriceRangeResponse()
                {
                    MinPrice = double.Parse(minPrice.ToString("0.##")),
                    MaxPrice = double.Parse(maxPrice.ToString("0.##"))
                };

                return new FilterResponse
                {
                    SizeResponseList = sizeResponse,
                    EventResponseList = eventResponse,
                    ReceivmentResponseList = recipientResponse,
                    FlowerResponseList = flowerResponse,
                    PriceRange = priceRangeResponse
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch filters");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }

        }
    }
}