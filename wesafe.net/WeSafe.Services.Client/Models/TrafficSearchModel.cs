using System;

namespace WeSafe.Services.Client.Models
{
    public class TrafficSearchModel
    {
        public int DeviceId { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }
    }
}
