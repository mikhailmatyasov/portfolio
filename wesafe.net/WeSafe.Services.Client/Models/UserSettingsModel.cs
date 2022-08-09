using System;
using System.Collections;
using System.Collections.Generic;

namespace WeSafe.Services.Client.Models
{
    public class UserSettingsModel
    {
        public DateTimeOffset? Mute { get; set; }

        public IEnumerable<SubscriberSettingsModel> Cameras { get; set; }
    }

    public class SubscriberSettingsModel
    {
        public int CameraId { get; set; }

        public DateTimeOffset? Mute { get; set; }
    }
}