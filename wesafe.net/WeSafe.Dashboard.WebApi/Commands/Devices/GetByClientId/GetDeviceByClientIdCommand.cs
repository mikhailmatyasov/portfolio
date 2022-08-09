using MediatR;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    /// <summary>
    /// Represents a command to get client device.
    /// </summary>
    public class GetDeviceByClientIdCommand : IRequest<DeviceModel>
    {
        /// <summary>
        /// Gets or sets a device id to search for.
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Gets or sets client id to search for.
        /// </summary>
        public int ClientId { get; set; }
    }
}