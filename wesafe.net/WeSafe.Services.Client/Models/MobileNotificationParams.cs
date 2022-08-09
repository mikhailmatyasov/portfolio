using System.Collections.Generic;

namespace WeSafe.Services.Client.Models
{
    public class MobileNotificationParams
    {
        public int DeviceId { get; set; }

        public int CameraId { get; set; }

        public int LogId { get; set; }

        public string Title { get; set; }

        public string NotificationText { get; set; }

        public IEnumerable<string> ImageUrls { get; set; }
    }
}