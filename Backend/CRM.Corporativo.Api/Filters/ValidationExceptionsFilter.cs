using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CRM.Corporativo.Domain.Base;

namespace CRM.Corporativo.Api.Filters;

public class ValidationExceptionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return;
        }

        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e =>
                e.Value?.Errors.Select(error => new TError(e.Key, error.ErrorMessage)) ?? Array.Empty<TError>())
            .ToList();

        var errorResponse = Result.Fail(errors);

        context.Result = new BadRequestObjectResult(errorResponse);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
