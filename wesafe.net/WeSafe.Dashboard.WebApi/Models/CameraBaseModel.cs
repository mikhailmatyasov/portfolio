namespace WeSafe.Dashboard.WebApi.Models
{
    public class CameraBaseModel
    {
        public string CameraName { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Ip { get; set; }

        public string Port { get; set; }

        public string SpecificRtcpConnectionString { get; set; }
    }
}
