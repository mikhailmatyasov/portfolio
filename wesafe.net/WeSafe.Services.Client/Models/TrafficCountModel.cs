namespace WeSafe.Services.Client.Models
{
    public class TrafficCountModel
    {
        public string CameraName { get; set; }

        public int CameraId { get; set; }

        public int Entered { get; set; }

        public int Exited { get; set; }
    }
}
