using FluentValidation;
using WeSafe.Authentication.WebApi.Commands.Users;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Authentication.WebApi.Validators.Users
{
    public class CreateUserCommandValidator : AbstractUserCommandValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(c => c.Password).ValidPassword();
        }
    }
}