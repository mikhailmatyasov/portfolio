using System;
using System.Collections.Generic;

namespace WeSafe.Services.Client.Models
{
    public class CameraLogModel : Model
    {
        public CameraLogModel()
        {
            Entries = new List<CameraLogEntryModel>();
        }

        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public int CameraId { get; set; }

        public string CameraName { get; set; }

        public bool Alert { get; set; }

        public string Parameters { get; set; }

        public string Message { get; set; }

        public DateTimeOffset Time { get; set; }

        public IEnumerable<CameraLogEntryModel> Entries { get; set; }
    }
}