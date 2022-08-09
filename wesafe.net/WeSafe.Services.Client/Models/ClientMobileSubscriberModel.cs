using System;
using System.Collections;
using System.Collections.Generic;

namespace WeSafe.Services.Client.Models
{
    public class ClientMobileSubscriberModel : ClientSubscriberModel
    {
        public DateTimeOffset? Mute { get; set; }

        public IEnumerable<CameraSettingsModel> CameraSettings { get; set; }

        public IEnumerable<MobileDeviceModel> Devices { get; set; }

        public IEnumerable<AssignmentModel> Assignments { get; set; }
    }
}