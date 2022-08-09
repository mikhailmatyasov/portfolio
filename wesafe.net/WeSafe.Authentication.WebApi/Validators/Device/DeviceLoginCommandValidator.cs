using FluentValidation;
using WeSafe.Authentication.WebApi.Commands.DeviceLogin;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Authentication.WebApi.Validators.Device
{
    /// <summary>
    /// Represents a device login command validation.
    /// </summary>
    public class DeviceLoginCommandValidator : AbstractValidator<DeviceLoginCommand>
    {
        #region Constructors

        public DeviceLoginCommandValidator()
        {
            RuleFor(c => c.MacAddress).NotEmpty().WithMessage("Device identifier is required");
            RuleFor(c => c.MacAddress).ValidMacAddress();
        }

        #endregion
    }
}