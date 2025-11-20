using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderService.BLL.FluentValidation;
using OrderService.BLL.Helpers;
using OrderService.BLL.Profiles;
using OrderService.BLL.Services;
using OrderService.BLL.Services.Interfaces;
using OrderService.DAL.Repositories;
using OrderService.DAL.Repositories.Interfaces;
using OrderService.DAL.UOW;
using OrderService.Domain.Database;
using OrderService.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// builder.AddServiceDefaults(); // Якщо використовуєте Aspire, тут має бути цей рядок

// Add services to the container.

builder.AddNpgsqlDbContext<OrderDbContext>("FlowerLabOrder");

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IGiftRepository, GiftRepository>();
builder.Services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<GiftCreateDtoValidator>(); 

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary")
);

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(OrderStatusProfile).Assembly);
});

builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();
builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddScoped<IOrderService, OrderServiceImpl>();

// --- NEW: Налаштування MassTransit (RabbitMQ) ---
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        // Отримуємо рядок підключення від Aspire або appsettings
        var rabbitMqConnection = builder.Configuration.GetConnectionString("rabbitmq"); 
        // Якщо Aspire немає, можна використати: cfg.Host("localhost", "/", h => { h.Username("guest"); h.Password("guest"); });
        
        if (!string.IsNullOrEmpty(rabbitMqConnection))
        {
             cfg.Host(rabbitMqConnection);
        }
        else 
        {
             // Fallback для локального запуску без Aspire
             cfg.Host("localhost", "/", h => {
                 h.Username("guest");
                 h.Password("guest");
             });
        }
    });
});

var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["Secret"];
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

var key = Encoding.UTF8.GetBytes(secretKey!);

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


var app = builder.Build();

// app.MapDefaultEndpoints(); // Для Aspire

using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

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