using System;
using WeSafe.Shared;

namespace WeSafe.Services.Client.Models
{
    public class EventBaseRequest: PageRequest
    {
        public string MobileId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
