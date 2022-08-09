using System;
using WeSafe.Shared;

namespace WeSafe.Services.Client.Models
{
    public class PlateEventSearchModel : PageRequest
    {
        public string PlateNumber { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }
    }
}
