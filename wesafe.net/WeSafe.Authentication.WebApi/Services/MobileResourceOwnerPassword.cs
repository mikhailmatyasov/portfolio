using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Commands.MobileUser;
using WeSafe.Authentication.WebApi.Enumerations;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Authentication.WebApi.Services.Abstract;

namespace WeSafe.Authentication.WebApi.Services
{
    /// <inheritdoc />
    public class MobileResourceOwnerPassword : IResourceOwnerPassword
    {
        private readonly IMediator _mediator;
        private ILogger<MobileResourceOwnerPassword> _logger;

        public MobileResourceOwnerPassword(IMediator mediator, ILogger<MobileResourceOwnerPassword> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        #region IResourceOwnerPassword

        /// <inheritdoc />
        public async Task Validate(ResourceOwnerPasswordValidationContext context)
        {
            var user = await GetMobileUser(context.UserName, CancellationToken.None);

            if (user == null)
            {
                await _mediator.Send(new CreateMobileUserCommand
                {
                    PhoneNumber = context.UserName,
                    Status = MobileUserStatus.Active
                }, CancellationToken.None);

                user = await GetMobileUser(context.UserName, CancellationToken.None);
            }

            if (user == null)
            {
                _logger.LogInformation($"Authentication failed for username: {context.UserName}");
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            context.Result = new GrantValidationResult(user.Phone, OidcConstants.AuthenticationMethods.Password);
        }

        #endregion

        #region Private Methods

        private async Task<MobileUserModel> GetMobileUser(string phone, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetMobileUserByPhoneCommand
            {
                PhoneNumber = phone
            }, cancellationToken);
            return user;
        }

        #endregion
    }
}
