using Aspire.Hosting;
using DotNetEnv;
using Projects;

Env.Load();

var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgres-password", secret: true);

var loginRabbitMQ = builder.AddParameter("rabbitmq-login", secret: true);
var passwordRabbitMQ = builder.AddParameter("rabbitmq-password", secret: true);

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

var rabbitmq = builder.AddRabbitMQ("rabbitmq", userName: loginRabbitMQ, password: passwordRabbitMQ)
    .WithManagementPlugin()
    .WithDataVolume("FlowerLabRabbitMQ");

var notificationService = builder.AddProject<Notify>("notification-bot")
    .WithReference(rabbitmq)
    .WithEnvironment("BOT__ID", Environment.GetEnvironmentVariable("BOT__ID"))
    .WithEnvironment("USER__ID", Environment.GetEnvironmentVariable("USER__ID"))
    .WaitFor(rabbitmq);

var catalogService = builder.AddProject<CatalogService_API>("catalog")
    .WithReference(postgresCatalog)
    .WithReference(rabbitmq)
    .WithReference(redis)
    .WithEnvironment("JWT_SECRET", Environment.GetEnvironmentVariable("JWT_SECRET"))
    .WithEnvironment("JWT_ISSUER", Environment.GetEnvironmentVariable("JWT_ISSUER"))
    .WithEnvironment("JWT_AUDIENCE", Environment.GetEnvironmentVariable("JWT_AUDIENCE"))
    .WithEnvironment("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES", Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES"))
    .WithEnvironment("JWT_REFRESH_TOKEN_EXPIRATION_DAYS", Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRATION_DAYS"))
    .WithEnvironment("CLOUDINARY_CLOUDNAME", Environment.GetEnvironmentVariable("CLOUDINARY_CLOUDNAME"))
    .WithEnvironment("CLOUDINARY_API_KEY", Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"))
    .WithEnvironment("CLOUDINARY_API_SECRET", Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET"))
    .WaitFor(postgresCatalog)
    .WaitFor(redis)
    .WaitFor(rabbitmq); 

var orderService = builder.AddProject<OrderService_API>("orders")
    .WithReference(postgresOrder)
    .WithReference(catalogService)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithEnvironment("JWT_SECRET", Environment.GetEnvironmentVariable("JWT_SECRET"))
    .WithEnvironment("JWT_ISSUER", Environment.GetEnvironmentVariable("JWT_ISSUER"))
    .WithEnvironment("JWT_AUDIENCE", Environment.GetEnvironmentVariable("JWT_AUDIENCE"))
    .WithEnvironment("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES", Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES"))
    .WithEnvironment("JWT_REFRESH_TOKEN_EXPIRATION_DAYS", Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRATION_DAYS"))
    .WithEnvironment("CLOUDINARY_CLOUDNAME", Environment.GetEnvironmentVariable("CLOUDINARY_CLOUDNAME"))
    .WithEnvironment("CLOUDINARY_API_KEY", Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"))
    .WithEnvironment("CLOUDINARY_API_SECRET", Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET"))
    .WithEnvironment("LIQPAY_PUBLIC_KEY", Environment.GetEnvironmentVariable("LIQPAY_PUBLIC_KEY"))
    .WithEnvironment("LIQPAY_PRIVATE_KEY", Environment.GetEnvironmentVariable("LIQPAY_PRIVATE_KEY"))
    .WithEnvironment("LIQPAY_SERVER_URL", Environment.GetEnvironmentVariable("LIQPAY_SERVER_URL"))
    .WithEnvironment("LIQPAY_SUCCESS_URL", Environment.GetEnvironmentVariable("LIQPAY_SUCCESS_URL"))
    .WaitFor(catalogService)
    .WaitFor(redis)
    .WaitFor(postgresOrder)
    .WaitFor(rabbitmq);

var reviewsService = builder.AddProject<ReviewService_API>("reviews")
    .WithReference(mongoReviews)
    .WithReference(catalogService)
    .WithReference(orderService)
    .WithReference(rabbitmq)
    .WithEnvironment("JWT_SECRET", Environment.GetEnvironmentVariable("JWT_SECRET"))
    .WithEnvironment("JWT_ISSUER", Environment.GetEnvironmentVariable("JWT_ISSUER"))
    .WithEnvironment("JWT_AUDIENCE", Environment.GetEnvironmentVariable("JWT_AUDIENCE"))
    .WithEnvironment("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES", Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES"))
    .WithEnvironment("JWT_REFRESH_TOKEN_EXPIRATION_DAYS", Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRATION_DAYS"))
    .WaitFor(mongoReviews)
    .WaitFor(catalogService)
    .WaitFor(orderService)
    .WaitFor(rabbitmq);

var userService = builder.AddProject<UsersService_API>("users")
    .WithReference(postgresUsers)
    .WithReference(rabbitmq)
    .WithEnvironment("JWT_SECRET", Environment.GetEnvironmentVariable("JWT_SECRET"))
    .WithEnvironment("JWT_ISSUER", Environment.GetEnvironmentVariable("JWT_ISSUER"))
    .WithEnvironment("JWT_AUDIENCE", Environment.GetEnvironmentVariable("JWT_AUDIENCE"))
    .WithEnvironment("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES", Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES"))
    .WithEnvironment("JWT_REFRESH_TOKEN_EXPIRATION_DAYS", Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRATION_DAYS"))
    .WithEnvironment("CLOUDINARY_CLOUDNAME", Environment.GetEnvironmentVariable("CLOUDINARY_CLOUDNAME"))
    .WithEnvironment("CLOUDINARY_API_KEY", Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"))
    .WithEnvironment("CLOUDINARY_API_SECRET", Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET"))
    .WithEnvironment("SENDGRID__KEY", Environment.GetEnvironmentVariable("SENDGRID__KEY"))
    .WithEnvironment("SENDGRID__FROM_EMAIL", Environment.GetEnvironmentVariable("SENDGRID__FROM_EMAIL"))
    .WithEnvironment("SENDGRID__FROM_NAME", Environment.GetEnvironmentVariable("SENDGRID__FROM_NAME"))
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