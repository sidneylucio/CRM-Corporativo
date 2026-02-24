using FluentValidation;
using CRM.Corporativo.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM.Corporativo.Api.Filters;

public class FluentValidationActionFilter<TModel, TValidator> : Attribute, IAsyncActionFilter
    where TValidator : IValidator<TModel>
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var validator = (TValidator)Activator.CreateInstance(typeof(TValidator))!;
        var model = context.ActionArguments.FirstOrDefault(arg => arg.Value is TModel).Value;

        if (model != null)
        {
            var validationResult = await validator.ValidateAsync(new ValidationContext<TModel>((TModel)model));
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(error => new TError(error.PropertyName, error.ErrorMessage)).ToList();

                var result = Result.Fail(errors);

                context.Result = new BadRequestObjectResult(result);
                return;
            }
        }

        await next();
    }
}
