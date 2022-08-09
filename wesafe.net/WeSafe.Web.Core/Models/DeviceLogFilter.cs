using System;
using System.Collections.Generic;

namespace WeSafe.Web.Core.Models
{
    public class DeviceLogFilter : IDeviceLogFilter
    {
        public string SearchText { get; set; }

        public int? ClientId { get; set; }

        public int? DeviceId { get; set; }

        public int? CameraId { get; set; }

        public IEnumerable<Shared.Enumerations.LogLevel?>  LogLevels { get; set; }

        public DateTimeOffset? FromDate { get; set; }

        public DateTimeOffset? ToDate { get; set; }
    }
}
