using FluentValidation;
using WeSafe.Dashboard.WebApi.Commands.Clients;

namespace WeSafe.Dashboard.WebApi.Validators.Client
{
    /// <summary>
    /// Represents a client creation command validation
    /// </summary>
    public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
    {
        #region Constructors

        public CreateClientCommandValidator()
        {
            RuleFor(c => c.Client).NotNull().WithMessage("The client can't be null.");
            RuleFor(c => c.Client).SetValidator(new ClientModelValidator());
        }

        #endregion
    }
}