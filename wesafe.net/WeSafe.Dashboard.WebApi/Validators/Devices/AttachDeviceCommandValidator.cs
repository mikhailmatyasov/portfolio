using FluentValidation;
using WeSafe.Dashboard.WebApi.Commands.Devices;

namespace WeSafe.Dashboard.WebApi.Validators.Devices
{
    /// <summary>
    /// Represents a attach device command validator.
    /// </summary>
    public class AttachDeviceCommandValidator : DeviceByTokenCommandValidator<AttachDeviceCommand>
    {
        public AttachDeviceCommandValidator()
        {
            RuleFor(c => c.ClientId).GreaterThan(0).WithMessage("Client identifier is required.");
        }
    }
}