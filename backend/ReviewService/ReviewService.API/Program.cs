using FluentValidation.AspNetCore;
using FluentValidation;
using MediatR;
using ReviewService.Application.Behaviours;
using ReviewService.Application.Features.Reviews.Commands.UpdateReview;
using ReviewService.Domain.Interfaces;
using ReviewService.Infrastructure.DB.Extension;
using ReviewService.Infrastructure.DB.HealthCheck;
using ReviewService.Infrastructure.DB.MappingConfig;
using ReviewService.Infrastructure.DB.Seeding;
using ReviewService.Infrastructure.Repositories;
using ReviewService.API.Middleware;
using System.Net.Sockets;
using ReviewService.Application.GrpcServer;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using MassTransit;
using ReviewService.Application.Consumers;
using ReviewService.Application.Validation.Reviews;
using DotNetEnv;
using shared.events;
using ReviewService.Application.Consumers.EventLog;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var secretKey = Env.GetString("JWT_SECRET") ?? throw new Exception("JWT_SECRET missing!");
var issuer = Env.GetString("JWT_ISSUER") ?? throw new Exception("JWT_ISSUER missing!");
var audience = Env.GetString("JWT_AUDIENCE") ?? throw new Exception("JWT_AUDIENCE missing!");
var accessTokenExpiration = int.Parse(Env.GetString("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES") ?? "15");
var refreshTokenExpiration = int.Parse(Env.GetString("JWT_REFRESH_TOKEN_EXPIRATION_DAYS") ?? "7");

BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
var pack = new ConventionPack
{
    new EnumRepresentationConvention(BsonType.String)
};
ConventionRegistry.Register("EnumStringConvention", pack, t => true);

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

ValueObjectMappings.Register();
builder.Services.AddGrpc();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BouquetDeletedConsumer>();

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

        cfg.ReceiveEndpoint("review-service-bouquet-events", e =>
        {
            e.Bind("bouquet-deleted-exchange", s =>
            {
                s.ExchangeType = "fanout";
            });

            e.ConfigureConsumer<BouquetDeletedConsumer>(context);
        });
    });
});

builder.AddMongoDBClient("FlowerLabReviews");
builder.Services.AddMongoDb();

builder.Services.AddHealthChecks()
    .AddCheck<MongoHealthCheck>("MongoDB");

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UpdateReviewWithIdCommandHandler>());

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<IEventLogService, EventLogService>();

builder.Services.AddScoped<ReviewsByBouquetServiceImpl>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(CreateReviewCommandValidator).Assembly);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

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

var catalogAddress = builder.Configuration["services:catalog:https:0"]
    ?? builder.Configuration["services:catalog:http:0"];

var orderAddress = builder.Configuration["services:orders:https:0"]
    ?? builder.Configuration["services:orders:http:0"];

if (string.IsNullOrEmpty(orderAddress))
{
    throw new InvalidOperationException("Не знайдено адресу order service");
}

builder.Services.AddGrpcClient<CheckOrderServiceGRPC.CheckOrderServiceGRPCClient>(options =>
{
    options.Address = new Uri(orderAddress);
}).ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
});

if (string.IsNullOrEmpty(catalogAddress))
{
    throw new InvalidOperationException("Не знайдено адресу catalog service");
}
else
{
    builder.Services.AddGrpcClient<CheckIdInReviews.CheckIdInReviewsClient>(options =>
    {
        options.Address = new Uri(catalogAddress);
    }).ConfigureChannel(channelOptions =>
    {
        channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
        channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
    });
}

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapGrpcService<ReviewsByBouquetServiceImpl>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    services.EnsureIndexes();

    List<IDataSeeder> seeders = new List<IDataSeeder>()
    {
        services.GetRequiredService<ReviewSeeder>(),
    };

    foreach (var seeder in seeders)
    {
        await seeder.SeedAsync();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();