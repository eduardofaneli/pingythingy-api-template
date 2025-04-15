using FluentValidation;
using PingyThingy.Api.Models;

namespace PingyThingy.Api.Validation;

public class WebhookPayloadValidator : AbstractValidator<WebhookPayloadDto>
{
    public WebhookPayloadValidator()
    {
        // Example validation rule: EventType is required
        RuleFor(x => x.EventType).NotEmpty().WithMessage("Event type cannot be empty.");

        // Add other validation rules based on your requirements
        // RuleFor(x => x.Data).NotNull();
        // RuleFor(x => x.SomeOtherProperty).MaximumLength(100);
    }
}
