using System;
using System.Collections.Generic;

namespace WeSafe.Web.Models
{
    public class MobileEventFilter
    {
        public int? Skip { get; set; }
        
        public int? Take { get; set; }
        
        public DateTime? FromDate { get; set; }
        
        public DateTime? ToDate { get; set; }
        
        public IEnumerable<int> DeviceIds { get; set; }

        public IEnumerable<int> CameraIds { get; set; }
    }
}