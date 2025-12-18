using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.DAL.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.Services
{
    public class ExpiredOrdersCleanupService : BackgroundService
    {
        private IServiceProvider serviceProvider;
        private ILogger<ExpiredOrdersCleanupService> logger;

        public ExpiredOrdersCleanupService(IServiceProvider serviceProvider, ILogger<ExpiredOrdersCleanupService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("ExpiredOrdersCleanupService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var now = DateTime.UtcNow;

                    var expiredOrders = await unitOfWork.Orders.GetExpiredAwaitingPaymentOrdersAsync(now, stoppingToken);

                    foreach (var order in expiredOrders)
                    {
                        logger.LogInformation($"Deleting expired order {order.Id}");

                        foreach (var reservation in order.Reservations)
                        {
                            unitOfWork.OrderReservations.Delete(reservation);
                        }

                        foreach (var giftReservation in order.GiftReservations)
                        {
                            unitOfWork.GiftReservations.Delete(giftReservation);
                        }

                        unitOfWork.Orders.Delete(order);
                    }

                    if (expiredOrders.Any()) await unitOfWork.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error cleaning expired orders.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            logger.LogInformation("ExpiredOrdersCleanupService stopped.");
        }
    }
}
