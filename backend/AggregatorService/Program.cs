using AggregatorService.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFilterGrpcClient, FilterGrpcClient>();

var catalogAddress = builder.Configuration["services:catalog:https:0"]
    ?? builder.Configuration["services:catalog:http:0"];

if (string.IsNullOrEmpty(catalogAddress))
{
    throw new InvalidOperationException("Не знайдено адресу catalog service");
}

builder.Services.AddGrpcClient<FilterService.FilterServiceClient>(options =>
{
    options.Address = new Uri(catalogAddress);
}).ConfigureChannel(channelOptions =>
{
    channelOptions.MaxReceiveMessageSize = 5 * 1024 * 1024;
    channelOptions.MaxSendMessageSize = 5 * 1024 * 1024;
});

var app = builder.Build();

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
