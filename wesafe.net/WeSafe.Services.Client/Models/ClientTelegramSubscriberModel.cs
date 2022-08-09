using System;
using System.Collections;
using System.Collections.Generic;

namespace WeSafe.Services.Client.Models
{
    public class ClientTelegramSubscriberModel : ClientSubscriberModel
    {
        public long TelegramId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset? Mute { get; set; }

        public IEnumerable<CameraSettingsModel> CameraSettings { get; set; }

        public IEnumerable<AssignmentModel> Assignments { get; set; }
    }
}