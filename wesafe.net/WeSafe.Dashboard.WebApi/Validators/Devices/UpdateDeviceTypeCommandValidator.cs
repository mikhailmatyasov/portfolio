using FluentValidation;
using WeSafe.Dashboard.WebApi.Commands.Devices;

namespace WeSafe.Dashboard.WebApi.Validators.Devices
{
    public class UpdateDeviceTypeCommandValidator : AbstractValidator<UpdateDeviceTypeCommand>
    {
        public UpdateDeviceTypeCommandValidator()
        {
            RuleFor(x => x.DeviceId).GreaterThan(0).WithMessage("Device identifier is required.");
        }
    }
}