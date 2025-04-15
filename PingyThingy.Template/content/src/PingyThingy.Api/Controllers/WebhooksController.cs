using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingyThingy.Api.Models;

namespace PingyThingy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WebhooksController : ControllerBase
{
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(ILogger<WebhooksController> logger)
    {
        _logger = logger;
    }

    // POST /api/webhooks
    [HttpPost]
    public IActionResult ReceiveWebhook([FromBody] WebhookPayloadDto payload)
    {
        // FluentValidation runs automatically due to [ApiController] and registration (next step)
        // If validation fails, a 400 Bad Request is returned automatically.

        // Log the received payload (consider logging specific properties instead of the whole object if sensitive)
        _logger.LogInformation("Webhook received for event type: {EventType}", payload.EventType);

        // TODO: Add logic here to validate the webhook source (if necessary, e.g., signature check)
        // TODO: Add logic to enqueue the payload for background processing (using PingyThingy.Core)

        // Acknowledge receipt immediately
        return Ok();
    }
}
