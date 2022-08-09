using System.Text.RegularExpressions;

namespace WeSafe.Web.Common.Constants
{
    /// <summary>
    /// Common constants
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Regex pattern for IP address
        /// </summary>
        public static readonly Regex IpAddressPattern = new Regex(@"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");

        /// <summary>
        /// Regex pattern for MAC address
        /// </summary>
        public static readonly Regex MacAddressPattern = new Regex(@"^([0-9A-Fa-f]{2}:){5}([0-9A-Fa-f]{2})$");

        /// <summary>
        /// Regex pattern for phone number. 11-12 nums, eg +32214589357
        /// </summary>
        public static readonly Regex PhoneNumberPattern = new Regex(@"^\+\d{11,12}$");

        public const string defaultUserName = "Anonymous";
    }
}
