using AggregatorService.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFilterGrpcClient, FilterGrpcClient>();
builder.Services.AddScoped<IBouquetWithReviewsService, BouquetServiceImpl>();   

var catalogAddress = builder.Configuration["services:catalog:https:0"] ?? builder.Configuration["services:catalog:http:0"];
var reviewsAddress = builder.Configuration["services:reviews:https:0"] ?? builder.Configuration["services:reviews:http:0"];

if (string.IsNullOrEmpty(catalogAddress))
{
    throw new InvalidOperationException("Address for catalog service not found!");
}
if (string.IsNullOrEmpty(reviewsAddress))
{
    throw new InvalidOperationException("Address for reviews service not found!");
}

builder.Services.AddGrpcClient<FilterService.FilterServiceClient>(options =>
{
    options.Address = new Uri(catalogAddress);
}).ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
});
builder.Services.AddGrpcClient<BouquetGrpcService.BouquetGrpcServiceClient>(options =>
{
    options.Address = new Uri(catalogAddress);
}).ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
});
builder.Services.AddGrpcClient<ReviewsByBouquetId.ReviewsByBouquetIdClient>(options =>
{
    options.Address = new Uri(reviewsAddress);
}).ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
});

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
