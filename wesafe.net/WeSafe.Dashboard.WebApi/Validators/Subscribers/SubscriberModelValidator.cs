using System;
using FluentValidation;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Dashboard.WebApi.Validators.Subscribers
{
    /// <summary>
    /// Represents a subscriber model validator
    /// </summary>
    public class SubscriberModelValidator : AbstractValidator<SubscriberModel>
    {
        public SubscriberModelValidator()
        {
            RuleFor(c => c.Phone).NotEmpty().WithMessage("Phone number is required");
            RuleFor(c => c.Phone).ValidPhoneNumber();
            RuleFor(c => c.Name).MaximumLength(200).WithMessage("Subscriber name is too long.").When(c => !String.IsNullOrEmpty(c.Name));
            RuleFor(c => c.ClientId).GreaterThan(0).WithMessage("Subscriber client identifier is required.");
        }
    }
}