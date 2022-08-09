using FluentValidation;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Dashboard.WebApi.Validators.Devices
{
    /// <summary>
    /// Represents a <see cref="GetDevicesByClientIdCommand"/> validator.
    /// </summary>
    public class GetDevicesByClientIdCommandValidator : AbstractValidator<GetDevicesByClientIdCommand>
    {
        public GetDevicesByClientIdCommandValidator()
        {
            RuleFor(x => x.ClientId).ValidClientId();
        }
    }
}