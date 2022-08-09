namespace WeSafe.Services.Client.Models
{
    public class CameraStatusModel
    {
        public int Id { get; set; }

        public string CameraName { get; set; }

        public string Status { get; set; }

        public string NetworkStatus { get; set; }
    }
}