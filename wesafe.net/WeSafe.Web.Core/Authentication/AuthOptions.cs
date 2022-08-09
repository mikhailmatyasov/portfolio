using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WeSafe.Web.Core.Authentication
{
    public class AuthOptions
    {
        private const string SecurityKey = "WeSafeSecurityKey|86675566";

        public static TimeSpan Lifetime => TimeSpan.FromMinutes(30);

        public static TimeSpan LifetimeMobile => TimeSpan.FromDays(365);

        public static TimeSpan LifetimeDesktop = TimeSpan.FromDays(365);

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecurityKey));
        }
    }
}