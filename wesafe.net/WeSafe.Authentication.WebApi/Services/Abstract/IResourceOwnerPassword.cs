using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace WeSafe.Authentication.WebApi.Services.Abstract
{
    /// <summary>
    /// Represents API to manage resources of user.
    /// </summary>
    public interface IResourceOwnerPassword
    {
        /// <summary>
        /// Validate that user exist.
        /// </summary>
        /// <param name="context">Identity Server validation context.</param>
        Task Validate(ResourceOwnerPasswordValidationContext context);
    }
}
