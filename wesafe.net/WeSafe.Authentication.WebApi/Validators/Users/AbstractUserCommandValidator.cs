using System;
using FluentValidation;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Authentication.Abstract;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Authentication.WebApi.Validators.Users
{
    public abstract class AbstractUserCommandValidator<TCommand> : AbstractUserNameValidator<TCommand> where TCommand : IUser
    {
        protected AbstractUserCommandValidator()
        {
            RuleFor(c => c.DisplayName).NotEmpty().WithMessage("Name is required");
            RuleFor(c => c.DisplayName).MaximumLength(250).WithMessage("Name is too long");

            RuleFor(c => c.Phone).ValidPhoneNumber().When(c => !String.IsNullOrEmpty(c.Phone));

            RuleFor(c => c.RoleName).NotNull().WithMessage("Role name is required.");
            RuleFor(c => c.RoleName).Must(roleName => roleName == UserRoles.Administrators || roleName == UserRoles.Users)
                                    .WithMessage("{PropertyValue} is not valid role name.");
        }
    }
}