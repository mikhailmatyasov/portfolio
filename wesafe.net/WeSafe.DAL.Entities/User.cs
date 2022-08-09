using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WeSafe.DAL.Entities
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool Demo { get; set; }

        public int? ClientId { get; set; }

        public virtual Client Client { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        public virtual ICollection<UnhandledException> Exceptions { get; set; }
    }
}