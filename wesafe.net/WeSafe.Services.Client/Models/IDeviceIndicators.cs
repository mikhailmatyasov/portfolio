using System.Collections.Generic;

namespace WeSafe.Services.Client.Models
{
    /// <summary>
    /// Provides an abstraction for device performance indicators.
    /// </summary>
    public interface IDeviceIndicators
    {
        /// <summary>
        /// The device CPU utilization in percents.
        /// </summary>
        double? CpuUtilization { get; }

        /// <summary>
        /// The device GPU utilization in percents.
        /// </summary>
        double? GpuUtilization { get; }

        /// <summary>
        /// The device memory utilization in percents.
        /// </summary>
        double? MemoryUtilization { get; }

        /// <summary>
        /// Temperature of the device chip in celsius.
        /// </summary>
        double? Temperature { get; }

        /// <summary>
        /// The device cameras FPS.
        /// </summary>
        Dictionary<int, double> CamerasFps { get; }

        /// <summary>
        /// Internet traffic of the device.
        /// </summary>
        double? Traffic { get; }
    }
}