using WeSafe.Shared;

namespace WeSafe.Services.Client.Models
{
    public class DeviceRequest : PageRequest
    {
        public enum FilterType
        {
            DeviceName,
            MACAddress,
            None
        }
        public int? ClientId { get; set; }
        public FilterType? FilterBy { get; set; }
    }
}