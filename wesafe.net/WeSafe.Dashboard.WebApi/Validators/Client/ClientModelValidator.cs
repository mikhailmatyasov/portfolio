using System;
using FluentValidation;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Web.Common.Validators;

namespace WeSafe.Dashboard.WebApi.Validators.Client
{
    /// <summary>
    /// Represents a client model validation.
    /// </summary>
    public class ClientModelValidator : AbstractValidator<ClientModel>
    {
        #region Constructors

        public ClientModelValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Client name is required")
                .MaximumLength(250)
                .WithMessage("Client name is too long.");
            RuleFor(c => c.Phone).NotEmpty().WithMessage("Phone number is required");
            RuleFor(c => c.Phone).ValidPhoneNumber();
        }

        #endregion
    }
}