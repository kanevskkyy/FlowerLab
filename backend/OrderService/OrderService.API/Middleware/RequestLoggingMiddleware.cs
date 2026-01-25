using System.Diagnostics;
using System.Net;
using OrderService.BLL.Exceptions;

namespace OrderService.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, context.Request.Path);

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation(
                    "Request completed in {ElapsedMilliseconds} ms with status code {StatusCode}",
                    stopwatch.ElapsedMilliseconds,
                    context.Response.StatusCode
                );
            }
        }
    }
}
