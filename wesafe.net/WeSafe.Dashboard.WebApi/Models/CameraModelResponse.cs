namespace WeSafe.Dashboard.WebApi.Models
{
    public class CameraModelResponse: CameraBaseModel
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }

        public string Roi { get; set; }

        public string Schedule { get; set; }

        public int DeviceId { get; set; }

        public string LastImagePath { get; set; }

        public string Status { get; set; }

        public string NetworkStatus { get; set; }

        public string RecognitionSettings { get; set; }
    }
}
