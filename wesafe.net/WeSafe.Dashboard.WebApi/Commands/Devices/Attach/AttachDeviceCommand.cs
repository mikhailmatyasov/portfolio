using MediatR;
using WeSafe.Dashboard.WebApi.Abstractions;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    /// <summary>
    /// Represents a command to attach device to the client.
    /// </summary>
    public class AttachDeviceCommand : IRequest, IDeviceByToken
    {
        /// <summary>
        /// Client identifier to attach to.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// The token of the attaching device.
        /// </summary>
        public string DeviceToken { get; set; }
    }
}