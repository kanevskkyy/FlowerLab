using CatalogService.BLL.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace CatalogService.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private RequestDelegate next;
        private ILogger<RequestLoggingMiddleware> logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, context.Request.Path);

            try
            {
                await next(context); 
            }
            // Removed catch block to let ExceptionHandlingMiddleware handle exceptions
            finally
            {
                stopwatch.Stop();
                logger.LogInformation("Request completed in {ElapsedMilliseconds} ms with status code {StatusCode}",
                    stopwatch.ElapsedMilliseconds, context.Response.StatusCode);
            }
        }
    }
}