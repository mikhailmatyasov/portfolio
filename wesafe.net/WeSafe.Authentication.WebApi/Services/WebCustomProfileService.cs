using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.Web.Common.Authentication;
using WeSafe.Web.Common.Authentication.Abstract;

namespace WeSafe.Authentication.WebApi.Services
{
    /// <inheritdoc />
    public class WebCustomProfileService : ICustomProfileService
    {
        private readonly IUserManager _userManager;

        public WebCustomProfileService(IUserManager userManager)
        {
            _userManager = userManager;
        }

        #region ICustomProfileService

        /// <inheritdoc />
        public async Task GetProfileData(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);

            var role = await _userManager.GetUserRole(user);

            var claims = new List<Claim>
            {
                new Claim(WeSafeClaimTypes.UserDisplayNameClaimType, user.DisplayName),
                new Claim(WeSafeClaimTypes.ClientIdClaimType, user.ClientId?.ToString()),
                new Claim(JwtClaimTypes.Role, role)
            };

            context.IssuedClaims.AddRange(claims);
        }

        /// <inheritdoc />
        public async Task IsActive(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);

            context.IsActive = (user != null) && user.IsActive;
        }

        #endregion
    }
}
