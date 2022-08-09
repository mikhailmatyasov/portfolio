using System;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Services.Client.Models
{
    public class PlateEventDisplayModel
    {
        public string CameraName { get; set; }

        public string FrameImageUrl { get; set; }

        public string PlateNumberImageUrl { get; set; }

        public string PlateNumberString { get; set; }

        public LicensePlateType? LicensePlateType { get; set; }

        public DateTime DateTime { get; set; }
    }
}
