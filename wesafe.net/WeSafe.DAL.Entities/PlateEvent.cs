using System;
using System.Collections.Generic;

namespace WeSafe.DAL.Entities
{
    public class PlateEvent
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public virtual Device Device { get; set; }

        public int CameraId { get; set; }

        public virtual Camera Camera { get; set; }

        public ICollection<Frame> Frames { get; set; }

        public string PlateNumber { get; set; }

        public ICollection<PlateEventState> PlateEventState { get; set; }

        public DateTime DateTime { get; set; }
    }
}
