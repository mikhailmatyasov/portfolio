using System;
using System.Collections.Generic;

namespace WeSafe.DAL.Entities
{
    public class Camera
    {
        public int Id { get; set; }

        public string CameraName { get; set; }

        public string Ip { get; set; }

        public string Port { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; }

        public string Roi { get; set; }

        public string Schedule { get; set; }

        /// <summary>
        /// if SpecificRtcpConnectionString is empty then RtcpConnectionString is "rtsp://{Login}:{Password}@{Ip}/live"
        /// if SpecificRtcpConnectionString is not empty then it will be used as is. For example "rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mov"
        /// </summary>
        public string SpecificRtcpConnectionString { get; set; }

        public int DeviceId { get; set; }

        public virtual Device Device { get; set; }

        public string LastImagePath { get; set; }

        public string Status { get; set; }

        public string NetworkStatus { get; set; }

        public ICollection<CameraLog> CameraLogs { get; set; }

        public string RecognitionSettings { get; set; }

        public ICollection<DeviceLog> DeviceLogs { get; set; }

        public DateTimeOffset? LastActivityTime { get; set; }

        public ICollection<TrafficEvent> Traffic { get; set; }

        public ICollection<ClientSubscriberAssignment> Assignments { get; set; }

        public ICollection<ClientSubscriberSettings> SubscriberSettings { get; set; }

        public string Metadata { get; set; }
    }
}