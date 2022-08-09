using FluentValidation;
using WeSafe.Dashboard.WebApi.Commands.Register;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Dashboard.WebApi.Validators.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(c => c.UserName).NotEmpty().WithMessage("Username is required");
            RuleFor(c => c.UserName).MaximumLength(50).WithMessage("Username is too long");

            RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(c => c.Password).ValidPassword();

            RuleFor(c => c.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(c => c.Name).MaximumLength(250).WithMessage("Name is too long");

            RuleFor(c => c.Phone).ValidPhoneNumber();

            RuleFor(c => c.DeviceToken).ValidDeviceToken();
        }
    }
}