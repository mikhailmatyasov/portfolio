using System.Collections.Generic;
using WeSafe.Bus.Contracts;
using WeSafe.Bus.Contracts.Event;

namespace WeSafe.Bus.Components.Models.Event
{
    public class CreateEventContract : ICreateEventContract
    {
        public string DeviceMacAddress { get; set; }

        public string CameraIp { get; set; }

        public int CameraId { get; set; }

        public string Alert { get; set; }

        public string Message { get; set; }

        public IEnumerable<Blob> Frames { get; set; }
    }
}
