using IdentityServer4.Models;
using System.Threading.Tasks;

namespace WeSafe.Authentication.WebApi.Services.Abstract
{
    /// <summary>
    /// Represent an API to fill IdentityServer profile data. 
    /// </summary>
    public interface ICustomProfileService
    {
        /// <summary>
        /// Fills claims and other profile data.
        /// </summary>
        /// <param name="context">Identity Server profile data request.</param>
        Task GetProfileData(ProfileDataRequestContext context);

        /// <summary>
        /// Fills IsActive flag for IsActiveContext if user is active.
        /// </summary>
        /// <param name="context">Identity Server active context.</param>
        Task IsActive(IsActiveContext context);
    }
}
