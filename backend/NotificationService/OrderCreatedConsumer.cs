using MassTransit;
using shared.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify
{
    public class OrderCreatedConsumer : IConsumer<TelegramOrderCreatedEvent>
    {
        private TelegramBotService telegramBot;

        public OrderCreatedConsumer(TelegramBotService telegramBot)
        {
            this.telegramBot = telegramBot;
        }

        public async Task Consume(ConsumeContext<TelegramOrderCreatedEvent> context)
        {
            var order = context.Message;

            string message = $"📦 *Нове замовлення!* \n" +
                             $"🆔 Замовлення: #{order.OrderId}\n" +
                             $"👤 Від: {order.CustomerName}\n" +
                             $"💰 Сума: {order.TotalPrice} грн";

            await telegramBot.SendMessageAsync(message);
        }
    }
}