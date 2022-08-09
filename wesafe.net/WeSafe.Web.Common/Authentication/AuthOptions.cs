using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace WeSafe.Web.Common.Authentication
{
    public class AuthOptions
    {
        private const string SecurityKey = "WeSafeSecurityKey|86675566";

        public static TimeSpan Lifetime => TimeSpan.FromMinutes(30);

        public static TimeSpan LifetimeMobile => TimeSpan.FromDays(365);

        public static TimeSpan LifetimeDesktop = TimeSpan.FromDays(365);

        public static TimeSpan LifetimeDevice = TimeSpan.FromDays(15);

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecurityKey));
        }
    }
}
