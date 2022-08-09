using System.Collections.Generic;
using MediatR;
using WeSafe.Dashboard.WebApi.Models;

namespace WeSafe.Dashboard.WebApi.Commands.Devices
{
    /// <summary>
    /// Represents a command to create cameras.
    /// </summary>
    public class CreateCamerasCommand : IRequest
    {
        /// <summary>
        /// The device mac address to which the cameras will be attached.
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// The camera collection.
        /// </summary>
        public IEnumerable<CameraBaseModel> Cameras { get; set; }
    }
}