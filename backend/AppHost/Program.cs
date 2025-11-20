using Microsoft.Extensions.Configuration;
using Projects;
using StackExchange.Redis;

var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgres-password", secret: true);

var postgres = builder.AddPostgres("flower-lab-postgres", password: postgresPassword)
    .WithImage("postgres:16")
    .WithDataVolume("flowerlab-mongodata");

var postgresCatalog = postgres.AddDatabase("FlowerLabCatalog");
var postgresOrder = postgres.AddDatabase("FlowerLabOrder");
var postgresUsers = postgres.AddDatabase("FlowerLabUsers");

var mongo = builder.AddMongoDB("flower-lab-mongo")
    .WithImage("mongo:7")
    .WithDataVolume("flowerlab-mongodata");

var mongoReviews = mongo.AddDatabase("FlowerLabReviews");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin(); // Додає адмінку на порті 15672

var catalogService = builder.AddProject<CatalogService_API>("catalog")
    .WithReference(postgresCatalog)
    .WithReference(rabbitmq)
    .WaitFor(postgresCatalog)
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
    .WithReference(rabbitmq)
    .WaitFor(mongoReviews)
    .WaitFor(catalogService)
    .WaitFor(rabbitmq);

var userService = builder.AddProject<UsersService_API>("users")
    .WithReference(postgresUsers)
    .WithReference(rabbitmq)
    .WaitFor(postgresUsers);

var gateway = builder.AddProject<Gateway>("gateway")
    .WithReference(catalogService)
    .WithReference(orderService)
    .WithReference(reviewsService)
    .WithReference(userService)
    .WithExternalHttpEndpoints();

var aggregator = builder.AddProject<AggregatorService>("aggregator")
    .WithReference(catalogService)
    .WithReference(reviewsService)
    .WaitFor(catalogService)
    .WaitFor(reviewsService);

builder.Build().Run();