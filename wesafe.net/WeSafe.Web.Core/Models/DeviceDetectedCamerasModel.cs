using System.Collections.Generic;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Models
{
    public class DeviceDetectedCamerasModel
    {
        public string MacAddress { get; set; }

        public IEnumerable<CreateDetectedCameraModel> Cameras { get; set; }
    }
}