using System.Net;
using System.Text.Json;
using FluentValidation;
using UsersService.BLL.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UsersService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private RequestDelegate next;
        private ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "Сталася непередбачена помилка.";
            object? errors = null;

            switch (exception)
            {
                case ValidationException validationEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Помилка валідації.";
                    errors = validationEx.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage });
                    logger.LogWarning("Validation error: {Errors}", errors);
                    break;

                case EntityNotFoundException notFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundEx.Message;
                    logger.LogWarning("Entity not found: {Message}", notFoundEx.Message);
                    break;

                case InvalidOperationException invalidOpEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidOpEx.Message;
                    logger.LogWarning("Invalid operation: {Message}", invalidOpEx.Message);
                    break;

                case UnauthorizedAccessException unauthorizedEx:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = unauthorizedEx.Message;
                    logger.LogWarning("Unauthorized access: {Message}", unauthorizedEx.Message);
                    break;

                default:
                    logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                    message = exception.Message; 
                    break;
            }

            var response = new
            {
                Status = (int)statusCode,
                Message = message,
                Errors = errors
            };

            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}