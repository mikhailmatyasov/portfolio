using System;
using WeSafe.Shared;

namespace WeSafe.Services.Client.Models
{
    public class UnhandledExceptionRecordQuery : PageRequest
    {
        public string UserName { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
