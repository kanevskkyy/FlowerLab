var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, httpClient) =>
    {
        httpClient.ConnectTimeout = TimeSpan.FromSeconds(10);
    });

builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.MapReverseProxy();

await app.RunAsync();