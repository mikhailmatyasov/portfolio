using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using MediatR;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Commands.MobileUser;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.Shared.Roles;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Authentication.WebApi.Services
{
    /// <inheritdoc />
    public class MobileCustomProfileService : ICustomProfileService
    {
        private readonly IMediator _mediator;

        public MobileCustomProfileService(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region ICustomProfileService

        /// <inheritdoc />
        public async Task GetProfileData(ProfileDataRequestContext context)
        {
            var user = await GetMobileUser(context.Subject.GetSubjectId(), CancellationToken.None);
            var claims = new List<Claim>
            {
                new Claim(WeSafeClaimTypes.UserDisplayNameClaimType, user.Phone),
                new Claim(JwtClaimTypes.Role, UserRoles.Users)
            };

            context.IssuedClaims.AddRange(claims);
        }

        /// <inheritdoc />
        public Task IsActive(IsActiveContext context)
        {
            return Task.FromResult(true);
        }

        #endregion

        #region Provate Methods

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
