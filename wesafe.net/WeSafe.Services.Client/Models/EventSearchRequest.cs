using System.Collections.Generic;

namespace WeSafe.Services.Client.Models
{
    public class EventSearchRequest : EventBaseRequest
    {
        public IEnumerable<int> DeviceIds { get; set; }

        public IEnumerable<int> CameraIds { get; set; }
    }
}
