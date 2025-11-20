using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ReviewService.Application.Behaviours
{
    public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger;

        public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (FluentValidation.ValidationException validationsErrors)
            {
                foreach (var failure in validationsErrors.Errors)
                {
                    logger.LogWarning(validationsErrors, "Помилка валідації у {RequestName}: Властивість {PropertyName}, Помилка: {ErrorMessage}", typeof(TRequest).Name, failure.PropertyName, failure.ErrorMessage);
                }
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Неперехоплена помилка у {RequestName}: {@Request}", typeof(TRequest).Name, request);
                throw;
            }
        }
    }
}
