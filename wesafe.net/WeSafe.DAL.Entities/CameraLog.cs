using System;
using System.Collections.Generic;

namespace WeSafe.DAL.Entities
{
    public class CameraLog
    {
        public int Id { get; set; }

        public int CameraId { get; set; }

        public virtual Camera Camera { get; set; }

        public bool Alert { get; set; }

        public string Parameters { get; set; }

        public string Message { get; set; }

        public DateTimeOffset Time { get; set; }

        public ICollection<CameraLogEntry> Entries { get; set; }
    }
}