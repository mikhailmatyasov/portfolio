using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories.Extended;

namespace WeSafe.DAL.Abstractions
{
    /// <summary>
    /// Provides an abstraction for repository which manages clients.
    /// </summary>
    public interface IClientRepository : IExtendedRepository<Client>
    {
        /// <summary>
        /// Finds a client with specified phone number.
        /// </summary>
        /// <param name="phone">The phone number to search for.</param>
        /// <param name="disableTracking">
        /// <c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>false</c>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the client matching
        /// the specified <paramref name="phone"/> if it exists, otherwise null.
        /// </returns>
        Task<Client> FindByPhoneAsync(string phone, bool disableTracking = false);
    }
}