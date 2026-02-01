using DotNetEnv;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using shared.events.EmailEvents;
using shared.events.EventService;
using shared.events.OrderEvents;
using shared.events.TelegramBotEvent;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using UsersService.API.Helpers;
using UsersService.API.Middleware;
using UsersService.BLL;
using UsersService.BLL.Consumers;
using UsersService.BLL.FluentValidation;
using UsersService.BLL.Helpers;
using UsersService.BLL.Models.Auth;
using UsersService.BLL.Services;
using UsersService.BLL.Services.Events;
using UsersService.BLL.Services.Interfaces;
using UsersService.DAL.DbContext;
using UsersService.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

Env.Load();

string jwtSecret = Env.GetString("JWT_SECRET") ?? throw new Exception("JWT_SECRET missing!");
string jwtIssuer = Env.GetString("JWT_ISSUER") ?? throw new Exception("JWT_ISSUER missing!");
string jwtAudience = Env.GetString("JWT_AUDIENCE") ?? throw new Exception("JWT_AUDIENCE missing!");
int accessTokenExpiration = int.Parse(Env.GetString("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES") ?? "15");
int refreshTokenExpiration = int.Parse(Env.GetString("JWT_REFRESH_TOKEN_EXPIRATION_DAYS") ?? "7");

string cloudName = Env.GetString("CLOUDINARY_CLOUDNAME") ?? throw new Exception("CLOUDINARY_CLOUDNAME missing!");
string cloudApiKey = Env.GetString("CLOUDINARY_API_KEY") ?? throw new Exception("CLOUDINARY_API_KEY missing!");
string cloudApiSecret = Env.GetString("CLOUDINARY_API_SECRET") ?? throw new Exception("CLOUDINARY_API_SECRET missing!");

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(48); 
});

// CORS
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

builder.Services.AddScoped<IEventLogService, EventLogService>();

// DbContext
builder.AddNpgsqlDbContext<ApplicationDbContext>("FlowerLabUsers");

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var key = Encoding.UTF8.GetBytes(jwtSecret);
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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Cloudinary
builder.Services.Configure<CloudinarySettings>(options =>
{
    options.CloudName = cloudName;
    options.ApiKey = cloudApiKey;
    options.ApiSecret = cloudApiSecret;
});


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

        cfg.ReceiveEndpoint("users-addreses-queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
            e.Bind("order-address-exchange", s =>
            {
                s.ExchangeType = "fanout";
            });
        });

        cfg.Message<UserRegisteredEvent>(m =>
        {
            m.SetEntityName("send-user-confirm-email-exchange");
        });

        cfg.Message<UserResetPasswordEvent>(m =>
        {
            m.SetEntityName("send-user-reset-password-email-exchange");
        });
    });
});


// DI
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserImageService, UserImageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IUserService, UserService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<RegistrationDtoValidator>();
        fv.ImplicitlyValidateChildProperties = true;
    });

// Swagger
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

builder.Services.Configure<JwtSettings>(options =>
{
    options.Secret = jwtSecret;
    options.Issuer = jwtIssuer;
    options.Audience = jwtAudience;
    options.AccessTokenExpirationMinutes = accessTokenExpiration;
    options.RefreshTokenExpirationDays = refreshTokenExpiration;
});


var app = builder.Build();
app.UseExceptionHandlingMiddleware();

// Міграції та seed
using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        await DbInitializer.SeedRolesAsync(serviceProvider);
        await DbInitializer.SeedAdminUserAsync(serviceProvider);
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();