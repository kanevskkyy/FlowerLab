using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrderService.BLL.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderService.API.Middleware
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

            string code = "INTERNAL_SERVER_ERROR";
            string message = "An internal server error occurred.";
            
            switch (exception)
            {
                case ValidationException valEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    code = valEx.Code;
                    message = valEx.Message;
                    break;
                case System.ComponentModel.DataAnnotations.ValidationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    code = "VALIDATION_ERROR";
                    message = exception.Message;
                    break;
                case NotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    code = "NOT_FOUND";
                    message = exception.Message;
                    break;
                case AlreadyExistsException:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    code = "ALREADY_EXISTS";
                    message = exception.Message;
                    break;
                default:
                    _logger.LogError(exception, "An unhandled exception has occurred in OrderService.");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    message = $"DEBUG ERROR: {exception.Message} | Stack: {exception.ToString()}"; // TEMPORARY DEBUG
                    break;
            }

            var response = new { code, message };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
