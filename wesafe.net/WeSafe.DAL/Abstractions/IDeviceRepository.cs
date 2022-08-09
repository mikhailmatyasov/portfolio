using System.Collections.Generic;
using System.Threading.Tasks;
using WeSafe.DAL.Entities;
using WeSafe.DAL.Repositories.Extended;

namespace WeSafe.DAL.Abstractions
{
    /// <summary>
    /// Provides an abstraction for repository which manages devices.
    /// </summary>
    public interface IDeviceRepository : IExtendedRepository<Device>
    {
        /// <summary>
        /// Finds a device with specified MAC address.
        /// </summary>
        /// <param name="macAddress">The MAC address to search for.</param>
        /// <param name="disableTracking">
        /// <c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>false</c>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the device matching
        /// the specified <paramref name="macAddress"/> if it exists, otherwise null.
        /// </returns>
        Task<Device> FindByMacAddressAsync(string macAddress, bool disableTracking = false);

        /// <summary>
        /// Finds a device with specified token.
        /// </summary>
        /// <param name="token">The token to search for.</param>
        /// <param name="disableTracking">
        /// <c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>false</c>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the device matching
        /// the specified <paramref name="token"/> if it exists, otherwise null.
        /// </returns>
        Task<Device> FindByTokenAsync(string token, bool disableTracking = false);

        /// <summary>
        /// Find all devices attached to the client specified by identifier.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        Task<IEnumerable<Device>> FindAllByClientIdAsync(int clientId);

        /// <summary>
        /// Find a device attached to the client specified by identifier.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns></returns>
        Task<Device> FindByClientIdAsync(int clientId, int deviceId);

        /// <summary>
        /// Find all devices by ids and returns device names only.
        /// </summary>
        /// <param name="ids">Devices identifiers.</param>
        /// <returns>Devices collection that contains names only.</returns>
        Task<IEnumerable<Device>> GetDevicesWithNames(IEnumerable<int> ids);
    }
}