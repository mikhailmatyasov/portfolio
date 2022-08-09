using FluentValidation;
using WeSafe.Authentication.WebApi.Commands.MobileUser;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Authentication.WebApi.Validators.MobileUser
{
    public class GetMobileUserByPhoneValidator : AbstractValidator<GetMobileUserByPhoneCommand>
    {
        public GetMobileUserByPhoneValidator()
        {
            RuleFor(c => c.PhoneNumber).NotEmpty().WithMessage("Phone number is required");
            RuleFor(c => c.PhoneNumber).ValidPhoneNumber();
        }
    }
}