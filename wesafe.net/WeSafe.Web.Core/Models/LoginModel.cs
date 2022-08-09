using System.ComponentModel.DataAnnotations;

namespace WeSafe.Web.Core.Models
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Origin { get; set; }
    }
}