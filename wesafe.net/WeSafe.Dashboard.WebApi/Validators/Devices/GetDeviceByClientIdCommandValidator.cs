using FluentValidation;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Dashboard.WebApi.Validators.Devices
{
    /// <summary>
    /// Represents a <see cref="GetDeviceByClientIdCommand"/> validator.
    /// </summary>
    public class GetDeviceByClientIdCommandValidator : AbstractValidator<GetDeviceByClientIdCommand>
    {
        public GetDeviceByClientIdCommandValidator()
        {
            RuleFor(x => x.ClientId).ValidClientId();
            RuleFor(x => x.DeviceId).ValidDeviceId();
        }
    }
}