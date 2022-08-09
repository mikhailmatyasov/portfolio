namespace WeSafe.Services.Client.Models
{
    public class DeviceAuthToken
    {
        public int DeviceId { get; set; }

        public string AuthToken { get; set; }

        public string MACAddress { get; set; }

        public string Name { get; set; }
    }
}