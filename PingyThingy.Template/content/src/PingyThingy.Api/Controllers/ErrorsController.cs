using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PingyThingy.Api.Controllers;

[ApiController]
[AllowAnonymous] // Allow access without authentication
[ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger UI
public class ErrorsController : ControllerBase
{
    private readonly ILogger<ErrorsController> _logger;

    public ErrorsController(ILogger<ErrorsController> logger)
    {
        _logger = logger;
    }

    // Route will be determined by UseExceptionHandler configuration in Program.cs
    [Route("/error")]
    public IActionResult HandleError()
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error; // The original exception

        // Log the exception details internally
        var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier; // Get correlation ID
        _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", traceId);

        // Return a standardized error response
        return Problem(
            title: "An unexpected error occurred.",
            detail: "An internal server error occurred. Please try again later or contact support.",
            statusCode: StatusCodes.Status500InternalServerError,
            extensions: new Dictionary<string, object?> { { "traceId", traceId } } // Include correlation ID
        );
    }
}
