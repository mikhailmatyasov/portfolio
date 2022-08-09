using MediatR;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    /// <summary>
    /// Represents a command to update device type.
    /// </summary>
    public class UpdateDeviceTypeCommand : IRequest
    {
        /// <summary>
        /// Gets or sets a device identifier to search for
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Gets or sets a device type to update.
        /// </summary>
        public DeviceType DeviceType { get; set; }
    }
}