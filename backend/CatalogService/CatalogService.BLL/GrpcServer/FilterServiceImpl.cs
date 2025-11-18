using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogService.DAL.UnitOfWork;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

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
                var receivments = await unitOfWork.Recipients.GetAllAsync();
                var flowers = await unitOfWork.Flowers.GetAllAsync();

                var sizeResponse = new SizeResponseList
                {
                    Sizes = { sizes.Select(s => new SizeResponse { 
                        Id = s.Id.ToString(), 
                        Name = s.Name }) 
                    }
                };

                var eventResponse = new EventResponseList
                {
                    Events = { 
                        events.Select(e => new EventResponse { 
                            Id = e.Id.ToString(), 
                            Name = e.Name 
                        }) 
                    }
                };

                var receivmentResponse = new ReceivmentResponseList
                {
                    Receivments = { 
                        receivments.Select(r => new ReceivmentResponse { 
                            Id = r.Id.ToString(), 
                            Name = r.Name 
                        }) 
                    }
                };

                var flowerResponse = new FlowerResponseList
                {
                    Flowers = { flowers.Select(f => new FlowerResponse
                {
                    Id = f.Id.ToString(),
                    Name = f.Name,
                    Color = f.Color,
                    Description = f.Description,
                    Quantity = f.Quantity
                }) }
                    };

                return new FilterResponse
                {
                    SizeResponseList = sizeResponse,
                    EventResponseList = eventResponse,
                    ReceivmentResponseList = receivmentResponse,
                    FlowerResponseList = flowerResponse
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
