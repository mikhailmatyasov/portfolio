using FluentValidation;
using Microsoft.EntityFrameworkCore.Internal;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Dashboard.WebApi.Validators.Camera
{
    public class CreateCamerasCommandValidator: AbstractValidator<CreateCamerasCommand>
    {
        public CreateCamerasCommandValidator()
        {
            RuleFor(c => c.MacAddress).NotEmpty().WithMessage("Mac address is required");
            RuleFor(c => c.Cameras).Must(c => c != null && c.Any()).WithMessage("At least one camera is required");
            
            RuleForEach(c => c.Cameras).SetValidator(new CameraBaseValidator());
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            if (instanceToValidate == null)
                throw new BadRequestException("The input model can't be null. Please support us.");

            base.EnsureInstanceNotNull(instanceToValidate);
        }
    }
}
