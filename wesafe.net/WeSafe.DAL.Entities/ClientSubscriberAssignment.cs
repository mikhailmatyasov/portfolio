namespace WeSafe.DAL.Entities
{
    public class ClientSubscriberAssignment
    {
        public int Id { get; set; }

        public int ClientSubscriberId { get; set; }

        public ClientSubscriber Subscriber { get; set; }

        public int DeviceId { get; set; }

        public Device Device { get; set; }

        public int? CameraId { get; set; }

        public Camera Camera { get; set; }
    }
}