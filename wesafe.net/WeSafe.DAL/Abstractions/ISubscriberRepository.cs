using System.Threading.Tasks;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories.Extended;

namespace WeSafe.DAL.Abstractions
{
    /// <summary>
    /// Provides an abstraction for repository which manages client subscribers.
    /// </summary>
    public interface ISubscriberRepository : IExtendedRepository<ClientSubscriber>
    {
        /// <summary>
        /// Checks if any active subscriber with the given phone number is exists.
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<bool> HasActiveClientsAsync(string phone);

        Task<bool> AnyAsync(int clientId, string phone);
    }
}