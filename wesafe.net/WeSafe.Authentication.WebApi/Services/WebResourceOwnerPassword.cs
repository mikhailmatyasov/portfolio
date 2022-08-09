using IdentityServer4.AspNetIdentity;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WeSafe.Authentication.WebApi.Services.Abstract;
using WeSafe.DAL.Entities;

namespace WeSafe.Authentication.WebApi.Services
{
    /// <inheritdoc />
    public class WebResourceOwnerPassword : ResourceOwnerPasswordValidator<User>, IResourceOwnerPassword
    {
        public WebResourceOwnerPassword(UserManager<User> userManager, SignInManager<User> signInManager,
            ILogger<ResourceOwnerPasswordValidator<User>> logger) : base(userManager, signInManager, logger)
        {
        }

        #region IResourceOwnerPassword

        /// <inheritdoc />
        public Task Validate(ResourceOwnerPasswordValidationContext context)
        {
            return base.ValidateAsync(context);
        }

        #endregion
    }
}
