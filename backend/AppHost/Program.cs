using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Projects;
using StackExchange.Redis;
using DotNetEnv;

Env.Load();

var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgres-password", secret: true);

var postgres = builder.AddPostgres("flower-lab-postgres", password: postgresPassword)
    .WithImage("postgres:16")
    .WithDataVolume("flowerlab-postgres");

var postgresCatalog = postgres.AddDatabase("FlowerLabCatalog");
var postgresOrder = postgres.AddDatabase("FlowerLabOrder");
var postgresUsers = postgres.AddDatabase("FlowerLabUsers");

var mongo = builder.AddMongoDB("flower-lab-mongo")
    .WithImage("mongo:7")
    .WithDataVolume("flowerlab-mongodata");

var redis = builder.AddRedis("flowerlab-redis")
    .WithDataVolume("flowerlab-redisdata");

var mongoReviews = mongo.AddDatabase("FlowerLabReviews");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithDataVolume("FlowerLabRabbitMQ");

var notificationService = builder.AddProject<Notify>("notification-bot")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

var catalogService = builder.AddProject<CatalogService_API>("catalog")
    .WithReference(postgresCatalog)
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WaitFor(postgresCatalog)
    .WaitFor(redis)
    .WaitFor(rabbitmq); 

var orderService = builder.AddProject<OrderService_API>("orders")
    .WithReference(postgresOrder)
    .WithReference(catalogService)
    .WithReference(rabbitmq)
    .WaitFor(catalogService)
    .WaitFor(postgresOrder)
    .WaitFor(rabbitmq);

var reviewsService = builder.AddProject<ReviewService_API>("reviews")
    .WithReference(mongoReviews)
    .WithReference(catalogService)
    .WithReference(orderService)
    .WithReference(rabbitmq)
    .WaitFor(mongoReviews)
    .WaitFor(catalogService)
    .WaitFor(orderService)
    .WaitFor(rabbitmq);

var userService = builder.AddProject<UsersService_API>("users")
    .WithReference(postgresUsers)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitFor(postgresUsers);

var gateway = builder.AddProject<Gateway>("gateway")
    .WithReference(catalogService)
    .WithReference(orderService)
    .WithReference(reviewsService)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(userService)
    .WithExternalHttpEndpoints();

var aggregator = builder.AddProject<AggregatorService>("aggregator")
    .WithReference(catalogService)
    .WithReference(reviewsService)
    .WithReference(redis)
    .WaitFor(catalogService)
    .WaitFor(redis)
    .WaitFor(reviewsService);

builder.Build().Run();