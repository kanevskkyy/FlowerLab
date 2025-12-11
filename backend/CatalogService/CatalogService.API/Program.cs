using CatalogService.API.Middleware;
using CatalogService.BLL.Automapper;
using CatalogService.BLL.DTO;
using CatalogService.BLL.GrpcServer;
using CatalogService.BLL.Services.Implementations;
using CatalogService.BLL.Services.Interfaces;
using CatalogService.BLL.Validators;
using CatalogService.DAL.Context;
using CatalogService.DAL.Repositories.Implementations;
using CatalogService.DAL.Repositories.Interfaces;
using CatalogService.DAL.UnitOfWork;
using CloudinaryDotNet;
using DotNetEnv;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using MassTransit;
using FlowerLab.Shared.Events;
using CatalogService.BLL.Consumers;
using shared.events;
using CatalogService.BLL.EventLogService;

namespace CatalogService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            Env.Load();
            var secretKey = Env.GetString("JWT_SECRET") ?? throw new Exception("JWT_SECRET missing!");
            var issuer = Env.GetString("JWT_ISSUER") ?? throw new Exception("JWT_ISSUER missing!");
            var audience = Env.GetString("JWT_AUDIENCE") ?? throw new Exception("JWT_AUDIENCE missing!");
            var accessTokenExpiration = int.Parse(Env.GetString("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES") ?? "15");
            var refreshTokenExpiration = int.Parse(Env.GetString("JWT_REFRESH_TOKEN_EXPIRATION_DAYS") ?? "7");

            var cloudName = Env.GetString("CLOUDINARY_CLOUDNAME") ?? throw new Exception("CLOUDINARY_CLOUDNAME missing!");
            var apiKey = Env.GetString("CLOUDINARY_API_KEY") ?? throw new Exception("CLOUDINARY_API_KEY missing!");
            var apiSecret = Env.GetString("CLOUDINARY_API_SECRET") ?? throw new Exception("CLOUDINARY_API_SECRET missing!");

            builder.Services.Configure<CloudSettings>(options =>
            {
                options.CloudName = cloudName;
                options.ApiKey = apiKey;
                options.ApiSecret = apiSecret;
            });

            builder.AddNpgsqlDbContext<CatalogDbContext>("FlowerLabCatalog");

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

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IFlowerRepository, FlowerRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ISizeRepository, SizeRepository>();
            builder.Services.AddScoped<IRecipientRepository, RecipientRepository>();
            builder.Services.AddScoped<IBouquetRepository, BouquetRepository>();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<ISizeService, SizeService>();
            builder.Services.AddScoped<IRecipientService, RecipientService>();
            builder.Services.AddScoped<IFlowerService, FlowerService>();
            builder.Services.AddScoped<IBouquetService, BouquetService>();

            builder.Services.AddScoped<IEventLogService, EventLogService>();

            // Реєстрація UnitOfWork
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IImageService, CloudinaryImageService>();

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

            builder.Services.AddGrpc();

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderCreatedConsumer>();

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

                    cfg.ReceiveEndpoint("catalog-order-created-queue", e =>
                    {
                        e.ConfigureConsumer<OrderCreatedConsumer>(context);
                        e.Bind("order-created-exchange", s =>
                        {
                            s.ExchangeType = "fanout";
                        });
                    });

                    cfg.Message<BouquetDeletedEvent>(m =>
                    {
                        m.SetEntityName("bouquet-deleted-exchange");
                    });
                    cfg.Publish<BouquetDeletedEvent>(p =>
                    {
                        p.ExchangeType = "fanout";
                    });
                });
            });

            builder.Services.AddScoped<CheckIdInReviewsService>();
            builder.Services.AddScoped<CheckOrderService>();
            builder.Services.AddScoped<BouquetServiceGrpc>(); 
            builder.Services.AddScoped<FilterServiceImpl>();

            var app = builder.Build();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseMiddleware<RequestLoggingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                db.Database.Migrate();

                await CatalogSeeder.SeedAsync(db);
            }

            app.MapGrpcService<BouquetServiceGrpc>(); 
            app.MapGrpcService<CheckIdInReviewsService>();
            app.MapGrpcService<CheckOrderService>();
            app.MapGrpcService<FilterServiceImpl>();
            app.UseHttpsRedirection();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}