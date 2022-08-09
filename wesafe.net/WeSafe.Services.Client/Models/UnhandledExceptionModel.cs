using System;

namespace WeSafe.Services.Client.Models
{
    public class UnhandledExceptionModel
    {
        public string UserName { get; set; }

        public string ErrorMessage { get; set; }

        public string StackTrace { get; set; }

        public DateTime DateTime { get; set; }
    }
}
