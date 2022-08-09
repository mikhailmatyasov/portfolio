using System.Collections.Generic;
using MediatR;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    /// <summary>
    /// Represents a command to get client devices.
    /// </summary>
    public class GetDevicesByClientIdCommand : IRequest<IEnumerable<DeviceModel>>
    {
        /// <summary>
        /// Gets or sets client id to search for.
        /// </summary>
        public int ClientId { get; set; }
    }
}