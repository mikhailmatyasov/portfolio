using System;
using System.ComponentModel.DataAnnotations;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Web.Core.Models
{
    public class DeviceLogPresentationModel
    {
        [Required]
        public int DeviceId { get; set; }

        public int? CameraId { get; set; }

        [Required]
        public LogLevel LogLevel { get; set; }

        [Required]
        public string ErrorMessage { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
    }
}


