using System.Collections.Generic;
using System.Linq;

namespace WeSafe.Services.Client.Models
{
    public class AssignmentModel : Model
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public int? CameraId { get; set; }

        public string CameraName { get; set; }
    }
}