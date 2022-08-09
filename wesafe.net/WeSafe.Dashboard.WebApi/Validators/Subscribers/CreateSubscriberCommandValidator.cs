using FluentValidation;
using WeSafe.Dashboard.WebApi.Commands.Subscribers;

namespace WeSafe.Dashboard.WebApi.Validators.Subscribers
{
    /// <summary>
    /// Represents a subscriber creation command validation.
    /// </summary>
    public class CreateSubscriberCommandValidator : AbstractValidator<CreateSubscriberCommand>
    {
        public CreateSubscriberCommandValidator()
        {
            RuleFor(c => c.Subscriber).NotNull().WithMessage("Subscriber is required.");
            RuleFor(c => c.Subscriber).SetValidator(new SubscriberModelValidator());
        }
    }
}