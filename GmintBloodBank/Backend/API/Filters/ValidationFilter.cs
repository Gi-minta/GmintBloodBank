using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public sealed class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value?.Errors.Select(x => x.ErrorMessage).ToArray() ?? Array.Empty<string>());

            context.Result = new ObjectResult(new
            {
                error = "Validation failed",
                statusCode = (int)HttpStatusCode.BadRequest,
                errors,
            })
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
