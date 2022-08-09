using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using WeSafe.Logger.Abstraction.Enums;

namespace WeSafe.Logger.MongoDbStorage.Models
{
    public class DeviceLogBsonModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public int? CameraId { get; set; }

        public string CameraName { get; set; }

        public WeSafeLogLevel LogLevel { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }
    }
}
