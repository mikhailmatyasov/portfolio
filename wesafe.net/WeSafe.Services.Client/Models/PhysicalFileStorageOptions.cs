using System;

namespace WeSafe.Services.Client.Models
{
    public class PhysicalFileStorageOptions
    {
        public PhysicalFileStorageOptions()
        {
            BaseUrl = String.Empty;
            RequestPath = String.Empty;
        }

        public string Root { get; set; }

        public string RequestPath { get; set; }

        public string BaseUrl { get; set; }
    }
}