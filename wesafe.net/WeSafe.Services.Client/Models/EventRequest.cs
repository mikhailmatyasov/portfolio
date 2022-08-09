namespace WeSafe.Services.Client.Models
{
    public class EventRequest : EventBaseRequest
    {
        public int? DeviceId { get; set; }

        public int? CameraId { get; set; }
    }
}