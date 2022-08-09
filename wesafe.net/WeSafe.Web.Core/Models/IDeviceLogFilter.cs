using System;
using System.Collections.Generic;

namespace WeSafe.Web.Core.Models
{
    public interface IDeviceLogFilter
    {
        string SearchText { get; set; }

        int? ClientId { get; set; }

        int? DeviceId { get; set; }

        int? CameraId { get; set; }

        IEnumerable<Shared.Enumerations.LogLevel?> LogLevels{ get; set; }

        DateTimeOffset? FromDate { get; set; }

        DateTimeOffset? ToDate { get; set; }
    }
}
