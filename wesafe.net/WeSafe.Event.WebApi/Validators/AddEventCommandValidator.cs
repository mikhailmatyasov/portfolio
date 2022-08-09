using FluentValidation;
using System.Linq;
using WeSafe.Event.WebApi.Commands.AddEvent;
using WeSafe.Web.Common.Exceptions;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Event.WebApi.Validators
{
    public class AddEventCommandValidator : AbstractValidator<AddEventCommand>
    {
        public AddEventCommandValidator()
        {
            RuleFor(x => x.CameraId).GreaterThan(0).WithMessage("Camera identifier is empty.");
            RuleFor(x => x.DeviceMacAddress).NotEmpty().WithMessage("Mac address is empty.").ValidMacAddress();
            RuleFor(x => x.Frames).NotNull().WithMessage("Frames collection is NULL.");
            RuleFor(x => x.Frames).Must(x => x.Any())
                .When(x => x.Frames != null)
                .WithMessage("Frames collection is empty.");
            RuleFor(x => x.CameraIp).ValidIpAddress().When(x => !string.IsNullOrWhiteSpace(x.CameraIp));
            RuleForEach(x => x.Frames).Must(x => x.Length > 0).When(x => x.Frames != null).WithMessage("Some image does not contain body.");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            if (instanceToValidate == null)
                throw new BadRequestException("One of log models is null.");

            base.EnsureInstanceNotNull(instanceToValidate);
        }
    }
}
