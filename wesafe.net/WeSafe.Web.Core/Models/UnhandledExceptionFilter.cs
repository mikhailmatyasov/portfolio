using System;

namespace WeSafe.Web.Core.Models
{
    public class UnhandledExceptionFilter : PageFilter
    {
        public string SearchText { get; set; }

        public string UserName { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
