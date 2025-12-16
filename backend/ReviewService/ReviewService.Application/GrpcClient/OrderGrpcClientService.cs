using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Application.GrpcClient
{
    public class OrderGrpcClientService
    {
        private CheckOrderServiceGRPC.CheckOrderServiceGRPCClient client;

        public OrderGrpcClientService(CheckOrderServiceGRPC.CheckOrderServiceGRPCClient client)
        {
            this.client = client;
        }

        public async Task<bool> HasUserOrderedBouquetAsync(Guid userId, Guid bouquetId)
        {
            var request = new UserOrderCheckRequestMessage
            {
                UserId = userId.ToString(),
                BouquetId = bouquetId.ToString()
            };

            var response = await client.HasUserOrderedBouquetAsync(request);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new InvalidOperationException(response.ErrorMessage);
            }

            return response.HasOrdered;
        }
    }
}
