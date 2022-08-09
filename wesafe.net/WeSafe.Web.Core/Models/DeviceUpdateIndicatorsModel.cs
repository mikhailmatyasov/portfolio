using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Models
{
    /// <inheritdoc cref="IDeviceIndicators"/>
    public class DeviceUpdateIndicatorsModel : IDeviceIndicators
    {
        /// <summary>
        /// The device MAC address.
        /// </summary>
        [BindProperty(Name = "mac")]
        public string MacAddress { get; set; }

        /// <summary>
        /// The device client network IP address.
        /// </summary>
        [BindProperty(Name = "ip_box")]
        public string IpAddress { get; set; }

        [BindProperty(Name = "cpu")]
        public double? CpuUtilization { get; set; }

        [BindProperty(Name = "gpu")]
        public double? GpuUtilization { get; set; }

        [BindProperty(Name = "memory")]
        public double? MemoryUtilization { get; set; }

        [BindProperty(Name = "temp")]
        public double? Temperature { get; set; }

        [BindProperty(Name = "fps")]
        public Dictionary<int, double> CamerasFps { get; set; }

        public double? Traffic { get; set; }
    }
}