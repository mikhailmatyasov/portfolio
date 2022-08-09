using System.ComponentModel.DataAnnotations;

namespace WeSafe.Web.Core.Models
{
    public class ProfileModel
    {
        [Required]
        [StringLength(250)]
        public string DisplayName { get; set; }

        public string OldPassword { get; set; }

        public string Password { get; set; }
    }
}