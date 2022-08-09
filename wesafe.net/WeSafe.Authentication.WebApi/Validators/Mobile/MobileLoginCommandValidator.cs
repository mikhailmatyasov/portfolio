using FluentValidation;
using WeSafe.Authentication.WebApi.Commands.MobileLogin;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Authentication.WebApi.Validators.Mobile
{
    /// <summary>
    /// Represents a mobile device login command validation.
    /// </summary>
    public class MobileLoginCommandValidator : AbstractValidator<MobileLoginCommand>
    {
        #region Constructors

        public MobileLoginCommandValidator()
        {
            RuleFor(c => c.PhoneNumber).NotEmpty().WithMessage("Phone number is required");
            RuleFor(c => c.PhoneNumber).ValidPhoneNumber();
        }

        #endregion
    }
}