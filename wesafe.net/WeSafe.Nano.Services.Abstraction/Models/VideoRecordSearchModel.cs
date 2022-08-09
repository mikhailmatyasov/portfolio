using System;

namespace WeSafe.Nano.Services.Abstraction.Models
{
    public class VideoRecordSearchModel
    {
        public int? CameraId { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }
    }
}
