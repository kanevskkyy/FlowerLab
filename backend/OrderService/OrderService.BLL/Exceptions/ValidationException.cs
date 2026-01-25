using System;

namespace OrderService.BLL.Exceptions
{
    public class ValidationException : Exception
    {
        public string Code { get; }
        public ValidationException(string message, string code = "VALIDATION_ERROR") : base(message) 
        {
            Code = code;
        }
    }
}
