using System.Net;
using System.Text.Json;
using FluentValidation;
using UsersService.BLL.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var statusCode = HttpStatusCode.InternalServerError; 
        var message = "An unexpected error occurred.";
        object errors = null;

        // Обробка специфічних помилок
        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Validation failed.";
                errors = validationException.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage });
                break;
            case EntityNotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = notFoundException.Message;
                break;
            // Можна додати обробку інших Identity помилок
            default:
                // Log the full exception details
                _logger.LogError(exception, "Unhandled exception occurred: {message}", exception.Message);
                break;
        }

        var errorDetails = new 
        {
            Status = (int)statusCode,
            Message = message,
            Errors = errors // Буде null для загальних помилок
        };

        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails));
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}