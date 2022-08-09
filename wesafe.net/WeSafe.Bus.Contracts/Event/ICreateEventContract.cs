using System.Collections.Generic;

namespace WeSafe.Bus.Contracts.Event
{
    public interface ICreateEventContract
    {
        string DeviceMacAddress { get; set; }
        string CameraIp { get; set; }

        int CameraId { get; set; }

        string Alert { get; set; }

        string Message { get; set; }

        IEnumerable<Blob> Frames { get; set; }
    }
}
