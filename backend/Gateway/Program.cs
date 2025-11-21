var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, httpClient) =>
    {
        httpClient.ConnectTimeout = TimeSpan.FromSeconds(10);
    });

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

app.MapReverseProxy();

await app.RunAsync();