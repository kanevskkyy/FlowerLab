using DotNetEnv;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notify;

class Program
{
    public static async Task Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                Env.Load();
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                string? botToken = Environment.GetEnvironmentVariable("BOT__ID");
                long adminChatId = long.Parse(Environment.GetEnvironmentVariable("USER__ID"));
                string? rabbitMqConnection = configuration.GetConnectionString("rabbitmq");

                services.AddSingleton(new TelegramBotService(botToken!, adminChatId));

                services.AddMassTransit(mt =>
                {
                    mt.AddConsumer<OrderCreatedConsumer>(); 

                    mt.UsingRabbitMq((ctx, cfg) =>
                    {
                        if (!string.IsNullOrEmpty(rabbitMqConnection))
                        {
                            cfg.Host(rabbitMqConnection);
                        }
                        else
                        {   
                            cfg.Host("localhost", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });
                        }

                        cfg.ReceiveEndpoint("order-created-queue", e =>
                        {
                            e.ConfigureConsumer<OrderCreatedConsumer>(ctx);
                            e.Bind("telegram-order-created-exchange");
                        });
                    });
                });
            })
            .Build();

        await host.RunAsync();
    }
}