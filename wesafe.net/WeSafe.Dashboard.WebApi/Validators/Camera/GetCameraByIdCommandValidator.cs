using FluentValidation;
using WeSafe.Dashboard.WebApi.Commands.Camera;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Dashboard.WebApi.Validators.Camera
{
    public class GetCameraByIdCommandValidator : AbstractValidator<GetCameraByIdCommand>
    {
        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            if (instanceToValidate == null)
                throw new BadRequestException("The input model can't be null. Please support us.");

            base.EnsureInstanceNotNull(instanceToValidate);
        }

        public GetCameraByIdCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Camera identifier is required.");
        }
    }
}
