namespace WeSafe.Services.Client.Models
{
    public class CameraModel : BaseCameraModel
    {
        public int DeviceId { get; set; }

        public string LastImagePath { get; set; }

        public string Status { get; set; }

        public string NetworkStatus { get; set; }

        public bool IsActiveScheduler { get; set; }
    }
}
