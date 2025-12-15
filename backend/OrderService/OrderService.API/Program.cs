using CatalogService.API.Middleware;
using DotNetEnv;
using FlowerLab.Shared.Events;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderService.BLL.DTOs;
using OrderService.BLL.FluentValidation;
using OrderService.BLL.Grpc;
using OrderService.BLL.Helpers;
using OrderService.BLL.Profiles;
using OrderService.BLL.Services;
using OrderService.BLL.Services.Interfaces;
using OrderService.DAL.DbContext;
using OrderService.DAL.Repositories;
using OrderService.DAL.Repositories.Interfaces;
using OrderService.DAL.UOW;
using OrderService.Domain.Database;
using shared.events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var secretKey = Env.GetString("JWT_SECRET") ?? throw new Exception("JWT_SECRET missing!");
var issuer = Env.GetString("JWT_ISSUER") ?? throw new Exception("JWT_ISSUER missing!");
var audience = Env.GetString("JWT_AUDIENCE") ?? throw new Exception("JWT_AUDIENCE missing!");
var accessTokenExpiration = int.Parse(Env.GetString("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES") ?? "15");
var refreshTokenExpiration = int.Parse(Env.GetString("JWT_REFRESH_TOKEN_EXPIRATION_DAYS") ?? "7");

var cloudName = Env.GetString("CLOUDINARY_CLOUDNAME") ?? throw new Exception("CLOUDINARY_CLOUDNAME missing!");
var apiKey = Env.GetString("CLOUDINARY_API_KEY") ?? throw new Exception("CLOUDINARY_API_KEY missing!");
var apiSecret = Env.GetString("CLOUDINARY_API_SECRET") ?? throw new Exception("CLOUDINARY_API_SECRET missing!");

var liqPayPublic = Env.GetString("LIQPAY_PUBLIC_KEY") ?? throw new Exception("LIQPAY_PUBLIC_KEY missing!");
var liqPayPrivate = Env.GetString("LIQPAY_PRIVATE_KEY") ?? throw new Exception("LIQPAY_PRIVATE_KEY missing!");
var liqPayServerUrl = Env.GetString("LIQPAY_SERVER_URL") ?? throw new Exception("LIQPAY_SERVER_URL missing!");

builder.AddNpgsqlDbContext<OrderDbContext>("FlowerLabOrder");

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IGiftRepository, GiftRepository>();
builder.Services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<GiftCreateDtoValidator>();

builder.Services.Configure<CloudinarySettings>(options =>
{
    options.CloudName = cloudName;
    options.ApiKey = apiKey;
    options.ApiSecret = apiSecret;
});

builder.Services.Configure<LiqPaySettings>(options =>
{
    options.PublicKey = liqPayPublic;
    options.PrivateKey = liqPayPrivate;
    options.ServerUrl = liqPayServerUrl;
});

builder.Services.AddScoped<ILiqPayService, LiqPayService>();


builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(OrderStatusProfile).Assembly);
});

builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();
builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddScoped<IOrderService, OrderServiceImpl>();

builder.Services.AddMassTransit(x =>
{
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

        cfg.Message<TelegramOrderCreatedEvent>(m =>
        {
            m.SetEntityName("telegram-order-created-exchange");
        });

        cfg.Message<OrderCreatedEvent>(m =>
        {
            m.SetEntityName("order-created-exchange"); 
        });
        cfg.Publish<OrderCreatedEvent>(p =>
        {
            p.ExchangeType = "fanout"; 
        });
    });
});

var key = Encoding.UTF8.GetBytes(secretKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var catalogAddress = builder.Configuration["services:catalog:https:0"]
    ?? builder.Configuration["services:catalog:http:0"];

if (string.IsNullOrEmpty(catalogAddress))
{ 
    throw new InvalidOperationException("Не знайдено адресу catalog service");
}

if (!string.IsNullOrEmpty(catalogAddress))
{
    builder.Services.AddGrpcClient<CheckOrder.CheckOrderClient>(options =>
    {
        options.Address = new Uri(catalogAddress);
    }).ConfigureChannel(channelOptions =>
    {
        channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
        channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
    });
}

builder.Services.AddGrpc();


var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);

app.MapGrpcService<CheckOrderServiceGRPCImpl>();

using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();

    await OrderSeeder.SeedAsync(db);
}

app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();
app.Run();