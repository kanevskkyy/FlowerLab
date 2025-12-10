using DotNetEnv;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using UsersService.API.Helpers;
using UsersService.BLL;
using UsersService.BLL.FluentValidation;
using UsersService.BLL.Helpers;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;
using UsersService.BLL.Services;
using UsersService.DAL.DbContext;
using UsersService.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

Env.Load();

var jwtSecret = Env.GetString("JWT_SECRET") ?? throw new Exception("JWT_SECRET missing!");
var jwtIssuer = Env.GetString("JWT_ISSUER") ?? throw new Exception("JWT_ISSUER missing!");
var jwtAudience = Env.GetString("JWT_AUDIENCE") ?? throw new Exception("JWT_AUDIENCE missing!");
var accessTokenExpiration = int.Parse(Env.GetString("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES") ?? "15");
var refreshTokenExpiration = int.Parse(Env.GetString("JWT_REFRESH_TOKEN_EXPIRATION_DAYS") ?? "7");

var cloudName = Env.GetString("CLOUDINARY_CLOUDNAME") ?? throw new Exception("CLOUDINARY_CLOUDNAME missing!");
var cloudApiKey = Env.GetString("CLOUDINARY_API_KEY") ?? throw new Exception("CLOUDINARY_API_KEY missing!");
var cloudApiSecret = Env.GetString("CLOUDINARY_API_SECRET") ?? throw new Exception("CLOUDINARY_API_SECRET missing!");

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
});

// Cloudinary
builder.Services.Configure<CloudinarySettings>(options =>
{
    options.CloudName = cloudName;
    options.ApiKey = cloudApiKey;
    options.ApiSecret = cloudApiSecret;
});

// DI
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserImageService, UserImageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();

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
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        Description = "Введіть дійсний токен: 'Bearer <Access Token>'"
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
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