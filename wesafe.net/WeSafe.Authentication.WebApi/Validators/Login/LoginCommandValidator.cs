using WeSafe.Authentication.WebApi.Commands.Login;
using WeSafe.Web.Common.Exceptions;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Authentication.WebApi.Validators.Login
{
    public class LoginCommandValidator : LoginModelValidator<LoginCommand>
    {
        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            if (instanceToValidate == null)
                throw new BadRequestException("The input model can't be null. Please support us.");

            base.EnsureInstanceNotNull(instanceToValidate);
        }

        public LoginCommandValidator()
        {
        }
    }
}
