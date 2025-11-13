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
using ReviewService.Application.Validation.Additional;
using ReviewService.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ValueObjectMappings.Register();
builder.Services.AddMongoDb(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck<MongoHealthCheck>("MongoDB");

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UpdateReviewWithIdCommandHandler>());

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(UserInfoValidator).Assembly);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
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

app.UseAuthorization();

app.MapControllers();

app.Run();
