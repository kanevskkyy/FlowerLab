// UsersService.API/Program.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersService.BLL.Interfaces;
using UsersService.BLL.Models;
using UsersService.BLL.Services;
using UsersService.DAL.DbContext;
using UsersService.Domain.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Text;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UsersService.API.Helpers; // Для ініціалізації БД
using Microsoft.AspNetCore.Cors;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using UsersService.BLL;
using UsersService.BLL.FluentValidation;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// 1. Конфігурація JWT та Identity
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            // Дозволяємо будь-якому джерелу, включаючи Swagger, надсилати запити
            policy.AllowAnyOrigin() 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.AddNpgsqlDbContext<ApplicationDbContext>("FlowerLabUsers");

// 3. Реєстрація ASP.NET Identity (Core)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 4. Налаштування JWT Authentication (ВАЖЛИВО для інших мікросервісів!)
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
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
});

// 5. Реєстрація BLL сервісів (DI)
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 6. Налаштування вимог до пароля
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true; 
});
// 1. Реєстрація AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 2. Реєстрація AddressService
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
// Додавання контролерів та Swagger
builder.Services.AddControllers()
    // 1. Додаємо Fluent Validation до конвеєра MVC
    .AddFluentValidation(fv => 
    {
        // 2. Реєструємо всі валідатори, знайдені в асемблеї, де знаходиться RegistrationDtoValidator (тобто в BLL)
        fv.RegisterValidatorsFromAssemblyContaining<RegistrationDtoValidator>();
        
        // Опціонально: зупиняти виконання при першій помилці валідації
        fv.ImplicitlyValidateChildProperties = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Додаємо схему безпеки, щоб дозволити використання Bearer Token
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer", // Вказуємо тип схеми
        Description = "Введіть дійсний токен: 'Bearer <Access Token>'"
    });

    // Додаємо вимоги безпеки для всіх ендпоїнтів
    options.OperationFilter<SecurityRequirementsOperationFilter>(); 
});

var app = builder.Build();
app.UseExceptionHandlingMiddleware();

using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
// 7. Ініціалізація БД (Створення ролей та Admin користувача)
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

// Налаштування HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
// Важливо: Authentication має бути перед UseAuthorization
app.UseAuthentication(); 
app.UseAuthorization(); 

app.MapControllers();

app.Run();