using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace WeSafe.Authentication.WebApi.Services.Abstract
{
    /// <summary>
    /// Represents API to create resource owner password.
    /// </summary>
    public interface IResourceOwnerPasswordFactory
    {
        /// <summary>
        /// Creates resource owner password.
        /// </summary>
        /// <param name="context">Identity Server validation context.</param>
        /// <returns></returns>
        Task<IResourceOwnerPassword> CreateIResourceOwnerPassword(ResourceOwnerPasswordValidationContext context);
    }
}
