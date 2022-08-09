using System;
using WeSafe.Shared.Enumerations;

namespace WeSafe.DAL.Entities
{
    public class DeviceLog
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public virtual Device Device { get; set; }

        public int? CameraId { get; set; }

        public virtual Camera Camera { get; set; }

        public LogLevel LogLevel { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime DateTime { get; set; }
    }
}
