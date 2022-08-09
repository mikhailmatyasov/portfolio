using System;
using Microsoft.AspNetCore.Http;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Services.Client.Models
{
    public class DetectedCameraModel : Model
    {
        public string Name { get; set; }

        public string Ip { get; set; }

        public string Port { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public int DeviceId { get; set; }

        public CameraState State { get; set; }

        public string DetectingMethod { get; set; }

        public string ConnectFailureText { get; set; }
    }

    public class CreateDetectedCameraModel
    {
        public string Name { get; set; }

        public string Ip { get; set; }

        public string Port { get; set; }

        public string DetectingMethod { get; set; }
    }

    public class ConnectingDetectedCameraModel : Model
    {
        public string Login { get; set; }

        public string Password { get; set; }
    }

    public class ConnectDetectingCameraModel : Model
    {
        public int DeviceId { get; set; }

        public string RtspConnection { get; set; }

        public IFormFile Frame { get; set; }
    }

    public class FailureDetectingCameraModel : Model
    {
        public int DeviceId { get; set; }

        public string FailureText { get; set; }
    }
}