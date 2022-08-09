using System.ComponentModel.DataAnnotations;
using WeSafe.Shared.Enumerations;

namespace WeSafe.Web.Core.Models
{
    public class SignUpModel : DeviceTokenModel
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Phone { get; set; }

        public DeviceType DeviceType { get; set; }

        public string TimeZone { get; set; }
    }

    public class UserNameModel
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
    }
}