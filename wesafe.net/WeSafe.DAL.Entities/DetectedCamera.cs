using WeSafe.Shared.Enumerations;

namespace WeSafe.DAL.Entities
{
    public class DetectedCamera
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Ip { get; set; }

        public string Port { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public int DeviceId { get; set; }

        public Device Device { get; set; }

        public CameraState State { get; set; }

        public string DetectingMethod { get; set; }

        public string ConnectFailureText { get; set; }
    }
}