using System;
using System.Collections.Generic;
using WeSafe.Shared;

namespace WeSafe.Services.Client.Models
{
    public class ClientDeviceLogRecordQuery : PageRequest
    {
        public int? ClientId { get; set; }

        public int? DeviceId { get; set; }

        public int? CameraId { get; set; }

        public IEnumerable<Shared.Enumerations.LogLevel?> LogLevels { get; set; }

        public DateTimeOffset? FromDate { get; set; }

        public DateTimeOffset? ToDate { get; set; }
    }
}
