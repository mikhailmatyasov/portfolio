using System.ComponentModel.DataAnnotations;

namespace WeSafe.Web.Core.Models
{
    public class DeviceTokenModel
    {
        [Required]
        public string DeviceToken { get; set; }
    }
}