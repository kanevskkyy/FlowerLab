using CatalogService.BLL.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace CatalogService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new { message = exception.Message };
            
            switch (exception)
            {
                case BadRequestException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case NotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case AlreadyExistsException:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
                default:
                    _logger.LogError(exception, "An unhandled exception has occurred.");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = new { message = "An internal server error occurred." };
                    break;
            }

            // In development, you might want to include stack trace in 500s too, but keep it simple for now.
            // If it's a known exception like BadRequest, the message is safe to show.
            if (context.Response.StatusCode != (int)HttpStatusCode.InternalServerError)
            {
                response = new { message = exception.Message };
            }

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
