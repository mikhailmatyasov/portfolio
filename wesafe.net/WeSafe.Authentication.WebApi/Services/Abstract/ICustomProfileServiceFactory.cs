using System.Threading.Tasks;

namespace WeSafe.Authentication.WebApi.Services.Abstract
{
    /// <summary>
    /// Represents API to create <see cref="ICustomProfileService"/>
    /// </summary>
    public interface ICustomProfileServiceFactory
    {
        /// <summary>
        /// Creates custom profile service.
        /// </summary>
        /// <param name="clientId">Identity Server client identifier.</param>
        /// <returns>Instance <see cref="ICustomProfileService"/>.</returns>
        Task<ICustomProfileService> CreateCustomProfileService(string clientId);
    }
}
