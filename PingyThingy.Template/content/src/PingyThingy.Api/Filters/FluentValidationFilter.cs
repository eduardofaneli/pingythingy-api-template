using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PingyThingy.Api.Filters;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        // Find parameters that have associated validators
        foreach (var parameter in context.ActionArguments)
        {
            if (parameter.Value == null)
                continue;

            // Attempt to get a validator from DI for the parameter's type
            var validatorType = typeof(IValidator<>).MakeGenericType(parameter.Value.GetType());
            if (
                context.HttpContext.RequestServices.GetService(validatorType)
                is IValidator validator
            )
            {
                // Create a validation context
                var validationContext = new ValidationContext<object>(parameter.Value);

                // Perform validation
                var validationResult = await validator.ValidateAsync(
                    validationContext,
                    context.HttpContext.RequestAborted
                );

                if (!validationResult.IsValid)
                {
                    // Add validation errors to ModelState
                    foreach (var error in validationResult.Errors)
                    {
                        context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
            }
        }

        // If ModelState is invalid (due to FluentValidation or DataAnnotations), return BadRequest
        if (!context.ModelState.IsValid)
        {
            // Use ProblemDetails for a standardized error response
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred.",
            };
            context.Result = new BadRequestObjectResult(problemDetails);
            return; // Short-circuit the pipeline
        }

        // If valid, continue to the action
        await next();
    }
}
