using System.Collections.Generic;

namespace WeSafe.Logger.Abstraction.Models
{
    public class DevicesNamesRequest
    {
        public IEnumerable<int> Ids { get; set; }
    }
}
