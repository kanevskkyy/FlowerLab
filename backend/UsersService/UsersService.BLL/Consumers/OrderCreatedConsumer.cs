using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using shared.events;
using shared.events.EventService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.DAL.DbContext;
using UsersService.Domain.Entities;

namespace UsersService.BLL.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderAddressEvent>
    {
        private ApplicationDbContext dbContext;
        private ILogger<OrderCreatedConsumer> logger;

        private IEventLogService eventLogService;

        public OrderCreatedConsumer(ApplicationDbContext context, IEventLogService eventLogService, ILogger<OrderCreatedConsumer> logger)
        {
            this.dbContext = context;
            this.eventLogService = eventLogService;
            this.logger = logger;
        }


        public async Task Consume(ConsumeContext<OrderAddressEvent> context)
        {
            var eventId = context.Message.EventId;

            if (await eventLogService.HasEventProcessedAsync(eventId))
            {
                logger.LogWarning("Event {EventId} has already been processed. Skipping…", eventId);
                return;
            }

            var orderAddressEvent = context.Message;

            var exist = await dbContext.Addresses
                .FirstOrDefaultAsync(p => p.UserId == orderAddressEvent.UserId
                                       && p.Address.ToLower() == orderAddressEvent.Address.ToLower());

            if (exist != null)
                return;

            var userHasAddresses = await dbContext.Addresses
                .AnyAsync(a => a.UserId == orderAddressEvent.UserId);

            var newAddress = new UserAddresses
            {
                UserId = orderAddressEvent.UserId,
                Address = orderAddressEvent.Address,
                IsDefault = !userHasAddresses 
            };

            await dbContext.AddAsync(newAddress);
            await dbContext.SaveChangesAsync();
        }
    }
}
