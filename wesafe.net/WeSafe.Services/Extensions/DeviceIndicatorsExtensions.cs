using WeSafe.Services.Client.Models;

namespace WeSafe.Services.Extensions
{
    public static class DeviceIndicatorsExtensions
    {
        public static bool IsEmpty(this IDeviceIndicators indicators)
        {
            return indicators.CpuUtilization == null
                   && indicators.GpuUtilization == null
                   && indicators.MemoryUtilization == null
                   && indicators.Temperature == null
                   && (indicators.CamerasFps == null || indicators.CamerasFps.Count == 0)
                   && indicators.Traffic == null;
        }
    }
}