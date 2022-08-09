namespace WeSafe.Services.Client.Models
{
    public class BaseCameraModel : Model
    {
        public string CameraName { get; set; }

        public string Ip { get; set; }

        public string Port { get; set; }

        public string SpecificRtcpConnectionString { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; }

        public string Roi { get; set; }

        public string Schedule { get; set; }

        public string RecognitionSettings { get; set; }
    }
}