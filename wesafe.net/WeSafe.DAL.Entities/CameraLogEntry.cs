using System;

namespace WeSafe.DAL.Entities
{
    public class CameraLogEntry
    {
        public int Id { get; set; }

        public int CameraLogId { get; set; }

        public CameraLog CameraLog { get; set; }

        public string TypeKey { get; set; }

        public string ImageUrl { get; set; }

        public DateTimeOffset? UrlExpiration { get; set; }
    }
}