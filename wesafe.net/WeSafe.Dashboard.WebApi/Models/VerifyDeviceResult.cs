using WeSafe.Dashboard.WebApi.Enumerations;

namespace WeSafe.Dashboard.WebApi.Models
{
    /// <summary>
    /// Represents a device verification status.
    /// </summary>
    public class VerifyDeviceResult
    {
        public DeviceVerificationStatus Status { get; set; }
    }
}