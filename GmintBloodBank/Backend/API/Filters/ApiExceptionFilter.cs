using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public sealed class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var statusCode = context.Exception switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            ArgumentException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError,
        };

        context.Result = new ObjectResult(new
        {
            error = context.Exception.Message,
            statusCode = (int)statusCode,
        })
        {
            StatusCode = (int)statusCode,
        };

        context.ExceptionHandled = true;
    }
}
