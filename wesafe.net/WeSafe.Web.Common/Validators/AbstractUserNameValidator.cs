using FluentValidation;
using WeSafe.Web.Common.Authentication.Abstract;

namespace WeSafe.Web.Common.Validators
{
    /// <summary>
    /// Provides a base validation for models with user name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractUserNameValidator<T> : AbstractValidator<T> where T : ISupportUserName
    {
        protected AbstractUserNameValidator()
        {
            RuleFor(c => c.UserName).NotEmpty().WithMessage("Username is required");
            RuleFor(c => c.UserName).MaximumLength(50).WithMessage("Username is too long");
        }
    }
}