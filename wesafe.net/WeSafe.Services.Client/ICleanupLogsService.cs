using System;
using System.Threading.Tasks;

namespace WeSafe.Services.Client
{
    /// <summary>
    /// Provides an abstraction for managing logs cleanup.
    /// </summary>
    public interface ICleanupLogsService
    {
        /// <summary>
        /// Deletes camera logs older that time period.
        /// </summary>
        /// <param name="time">Time that defines period of life log.</param>
        Task DeleteCameraLogsOlderThan(TimeSpan time);

        /// <summary>
        /// Deletes device logs older that time period.
        /// </summary>
        /// <param name="time">Time that defines period of life log.</param>
        Task DeleteDeviceLogsOlderThan(TimeSpan time);
    }
}