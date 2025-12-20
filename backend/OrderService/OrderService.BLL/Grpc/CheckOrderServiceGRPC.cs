using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using OrderService.BLL.Services.Interfaces;

namespace OrderService.BLL.Grpc
{
    public class CheckOrderServiceGRPCImpl : CheckOrderServiceGRPC.CheckOrderServiceGRPCBase
    {
        private IOrderService orderService;

        public CheckOrderServiceGRPCImpl(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public override async Task<UserOrderCheckResponseMessage> HasUserOrderedBouquet(
            UserOrderCheckRequestMessage request, ServerCallContext context)
        {
            var response = new UserOrderCheckResponseMessage();

            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.BouquetId))
            {
                response.HasOrdered = false;
                response.ErrorMessage = "UserId or BouquetId was not provided.";
                return response;
            }

            try
            {
                bool hasOrdered = await orderService.HasUserOrderedBouquetAsync(
                    Guid.Parse(request.UserId), Guid.Parse(request.BouquetId));

                response.HasOrdered = hasOrdered;
                response.ErrorMessage = "";
            }
            catch (Exception ex)
            {
                response.HasOrdered = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
