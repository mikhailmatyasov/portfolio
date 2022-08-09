using FluentValidation;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Web.Common.Validators
{
    /// <summary>
    /// Provides a base validation for login models.
    /// </summary>
    /// <typeparam name="T">The login model. Should be inherited from <see cref="LoginModel"/>.</typeparam>
    public abstract class LoginModelValidator<T> : AbstractUserNameValidator<T> where T : LoginModel
    {
        protected LoginModelValidator()
        {
            RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(c => c.Password).ValidPassword();
        }
    }

    /// <summary>
    /// Represents a <see cref="LoginModel"/> validation.
    /// </summary>
    public class LoginModelValidator : LoginModelValidator<LoginModel>
    {
    }
}