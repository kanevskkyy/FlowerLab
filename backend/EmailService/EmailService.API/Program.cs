using EmailService.BLL.Consumers;
using EmailService.BLL.Service;
using EmailService.BLL.Service.Interfaces;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailProviderOptions>(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("SENDGRID__KEY");
    options.FromEmail = Environment.GetEnvironmentVariable("SENDGRID__FROM_EMAIL");
    options.FromName = Environment.GetEnvironmentVariable("SENDGRID__FROM_NAME");
});

builder.Services.AddScoped<IEmailService, SendGridEmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SendVerifyEmailEventConsumer>();
    x.AddConsumer<SendResetPasswordEmailEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConnection = builder.Configuration.GetConnectionString("rabbitmq");
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

        cfg.ReceiveEndpoint("send-user-reset-password-email-queue", e =>
        {
            e.ConfigureConsumer<SendResetPasswordEmailEventConsumer>(context);
            e.Bind("send-user-reset-password-email-exchange");
        });

        cfg.ReceiveEndpoint("send-user-confirm-email-queue", e =>
        {
            e.ConfigureConsumer<SendVerifyEmailEventConsumer>(context);
            e.Bind("send-user-confirm-email-exchange");
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
