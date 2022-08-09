using FluentValidation;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Web.Common.Exceptions;

namespace WeSafe.Dashboard.WebApi.Validators.Camera
{
    public class CameraBaseValidator : AbstractValidator<CameraBaseModel>
    {
        public CameraBaseValidator()
        {
            RuleFor(c => c.CameraName).NotEmpty().WithMessage("Camera name is required");
            RuleFor(c => c.CameraName).MaximumLength(250).WithMessage("Camera name is too long");

            RuleFor(c => c.Ip).NotEmpty().WithMessage("Camera IP is required");
            RuleFor(c => c.Ip).MaximumLength(16).WithMessage("Camera IP is too long");

            RuleFor(c => c.Port).NotEmpty().WithMessage("Camera port is required");
            RuleFor(c => c.Port).MaximumLength(10).WithMessage("Camera port is too long");

            RuleFor(c => c.Login).NotEmpty().WithMessage("Camera login is required");
            RuleFor(c => c.Login).MaximumLength(100).WithMessage("Camera login is too long");

            RuleFor(c => c.Password).NotEmpty().WithMessage("Camera password is required");
            RuleFor(c => c.Password).MaximumLength(50).WithMessage("Camera password is too long");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            if (instanceToValidate == null)
                throw new BadRequestException("The input model can't be null. Please support us.");

            base.EnsureInstanceNotNull(instanceToValidate);
        }
    }
}
