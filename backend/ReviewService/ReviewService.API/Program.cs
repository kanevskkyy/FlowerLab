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
using System.Net.Sockets;
using ReviewService.Application.GrpcServer;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson;

var builder = WebApplication.CreateBuilder(args);

// Налаштування MongoDB GUID serialization
BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
var pack = new ConventionPack
{
    new EnumRepresentationConvention(BsonType.String)
};
ConventionRegistry.Register("EnumStringConvention", pack, t => true);

ValueObjectMappings.Register();
builder.Services.AddGrpc();


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

builder.Services.AddScoped<ReviewsByBouquetServiceImpl>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(UserInfoValidator).Assembly);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var catalogAddress = builder.Configuration["services:catalog:https:0"]
    ?? builder.Configuration["services:catalog:http:0"];

if (string.IsNullOrEmpty(catalogAddress))
{
    throw new InvalidOperationException("Не знайдено адресу catalog service");
}

builder.Services.AddGrpcClient<CheckIdInReviews.CheckIdInReviewsClient>(options =>
{
    options.Address = new Uri(catalogAddress);
}).ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
});


var app = builder.Build();

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
app.UseAuthorization();
app.MapControllers();

app.Run();