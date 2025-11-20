using System.Diagnostics;

namespace CatalogService.API.Middleware
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

            _logger.LogInformation("Вхідний запит: {Method} {Path}", context.Request.Method, context.Request.Path);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Непередбачена помилка");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Запит завершено за {ElapsedMilliseconds} мс зі статусом {StatusCode}",
                    stopwatch.ElapsedMilliseconds, context.Response.StatusCode);
            }
        }
    }
}
