using FluentValidation;
using WeSafe.Dashboard.WebApi.Commands.Camera;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Dashboard.WebApi.Validators.Camera
{
    public class GetCameraNamesByIdCommandValidator : AbstractValidator<GetCameraNamesByIdCommand>
    {
        public GetCameraNamesByIdCommandValidator()
        {
            RuleFor(x => x.Ids).NotNull().WithMessage("Camera identifiers is null.");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            if (instanceToValidate == null)
                throw new BadRequestException("The input model can't be null. Please support us.");

            base.EnsureInstanceNotNull(instanceToValidate);
        }
    }
}
