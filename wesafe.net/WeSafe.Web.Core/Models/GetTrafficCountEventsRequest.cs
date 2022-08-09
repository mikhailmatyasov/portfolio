using Microsoft.AspNetCore.Mvc;
using System;

namespace WeSafe.Web.Core.Models
{
    [BindProperties(SupportsGet = true)]
    public class GetTrafficCountEventsRequest
    {
        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }
    }
}
