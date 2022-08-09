using System;
using Microsoft.AspNetCore.Http;

namespace WeSafe.Services.Client.Models
{
    public class PlateEventModel
    {
        public string DeviceMac { get; set; }

        public string CameraIp { get; set; }
        
        public IFormFile FrameImage { get; set; }

        public IFormFile PlateImage { get; set; }

        public string PlateNumber { get; set; }

        public DateTime DateTime { get; set; }
    }
}
