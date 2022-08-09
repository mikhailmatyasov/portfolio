using System;

namespace WeSafe.DAL.Entities
{
    public class ClientSubscriberSettings
    {
        public int Id { get; set; }

        public int ClientSubscriberId { get; set; }

        public ClientSubscriber ClientSubscriber { get; set; }

        public int CameraId { get; set; }

        public Camera Camera { get; set; }

        public DateTimeOffset? Mute { get; set; }
    }
}