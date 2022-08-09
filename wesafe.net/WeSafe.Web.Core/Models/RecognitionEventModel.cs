using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WeSafe.Web.Core.Models
{
    public class RecognitionEventBaseModel
    {
        [Required]
        [BindProperty(Name = "mac")]
        public string DeviceMAC { get; set; }

        [Required]
        [BindProperty(Name = "ip_cam")]
        public string CameraIP { get; set; }

        public int CameraId { get; set; }

        [BindProperty]
        public string Alert { get; set; }

        [BindProperty]
        public string Message { get; set; }
    }

    public class RecognitionEventModel : RecognitionEventBaseModel
    {
        [BindProperty]
        public IFormFile Frame { get; set; }
    }

    public class RecognitionEventsModel : RecognitionEventBaseModel
    {
        [BindProperty]
        public IFormFileCollection Frames { get; set; }
    }
}