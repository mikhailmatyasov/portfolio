using FluentValidation;
using WeSafe.Dashboard.WebApi.Abstractions;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Dashboard.WebApi.Validators.Devices
{
    /// <summary>
    /// Provides base validation for command that support searching device by the token.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public abstract class DeviceByTokenCommandValidator<TCommand> : AbstractValidator<TCommand> where TCommand : IDeviceByToken
    {
        protected DeviceByTokenCommandValidator()
        {
            RuleFor(c => c.DeviceToken).ValidDeviceToken();
        }
    }
}