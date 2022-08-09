using FluentValidation;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Dashboard.WebApi.Validators.Devices
{
    public class GetDevicesNamesByIdCommandValidator : AbstractValidator<GetDevicesNamesByIdCommand>
    {
        public GetDevicesNamesByIdCommandValidator()
        {
            RuleFor(x => x.Ids).NotNull().WithMessage("Device identifiers is null.");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            if (instanceToValidate == null)
                throw new BadRequestException("The input model can't be null. Please support us.");

            base.EnsureInstanceNotNull(instanceToValidate);
        }
    }
}
