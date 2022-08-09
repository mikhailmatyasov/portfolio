using MediatR;
using WeSafe.Dashboard.WebApi.Abstractions;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    /// <summary>
    /// Represents a command to find device by the token.
    /// </summary>
    public class GetDeviceByTokenCommand : IRequest<DeviceModel>, IDeviceByToken
    {
        /// <summary>
        /// Gets or sets a device token to search for.
        /// </summary>
        public string DeviceToken { get; set; }
    }
}