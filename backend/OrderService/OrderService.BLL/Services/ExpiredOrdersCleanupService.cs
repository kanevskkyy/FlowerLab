using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.DAL.UOW;
using OrderService.Domain.Entities;
using shared.cache;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.BLL.Services
{
    public class ExpiredOrdersCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<ExpiredOrdersCleanupService> logger;
        public ExpiredOrdersCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<ExpiredOrdersCleanupService> logger)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("ExpiredOrdersCleanupService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var unitOfWork =scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var cacheInvalidationService = scope.ServiceProvider.GetRequiredService<IEntityCacheInvalidationService<Gift>>();

                    var now = DateTime.UtcNow;

                    var expiredOrders = await unitOfWork.Orders.GetExpiredAwaitingPaymentOrdersAsync(now, stoppingToken);

                    foreach (var order in expiredOrders)
                    {
                        logger.LogInformation("Deleting expired order {OrderId}", order.Id);

                        foreach (var reservation in order.Reservations)
                        {
                            unitOfWork.OrderReservations.Delete(reservation);
                        }

                        foreach (var giftReservation in order.GiftReservations)
                        {
                            unitOfWork.GiftReservations.Delete(giftReservation);
                            await cacheInvalidationService.InvalidateByIdAsync(giftReservation.GiftId);
                        }

                        unitOfWork.Orders.Delete(order);
                    }

                    if (expiredOrders.Any())
                    {
                        await unitOfWork.SaveChangesAsync(stoppingToken);
                        await cacheInvalidationService.InvalidateAllAsync();
                    }
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
