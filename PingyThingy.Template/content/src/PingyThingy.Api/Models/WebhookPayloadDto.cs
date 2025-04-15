using System.Text.Json.Serialization;

namespace PingyThingy.Api.Models;

public class WebhookPayloadDto
{
    // Example property - replace with actual expected properties
    [JsonPropertyName("event_type")]
    public string? EventType { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; } // Use a more specific type if possible

    // Add other properties expected in the webhook payload
}
