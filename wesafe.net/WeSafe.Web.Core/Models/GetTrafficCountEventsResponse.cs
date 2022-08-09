using System.Collections.Generic;
using WeSafe.Services.Client.Models;

namespace WeSafe.Web.Core.Models
{
    public class GetTrafficCountEventsResponse
    {
        public IEnumerable<TrafficCountModel> Traffic { get; set; }
    }
}
