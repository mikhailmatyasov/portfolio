using System.Threading.Tasks;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories.Extended;

namespace WeSafe.DAL.Abstractions
{
    /// <summary>
    /// Provides an abstraction for repository which manages mobile users.
    /// </summary>
    public interface IMobileUserRepository : IExtendedRepository<MobileUser>
    {
        /// <summary>
        /// Finds a mobile user with specified phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number to search for.</param>
        /// <param name="disableTracking">
        /// <c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>false</c>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the mobile user matching
        /// the specified <paramref name="phoneNumber"/> if it exists, otherwise null.
        /// </returns>
        Task<MobileUser> FindByPhoneNumberAsync(string phoneNumber, bool disableTracking = false);
    }
}