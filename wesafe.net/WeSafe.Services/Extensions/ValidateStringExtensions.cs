using System.Text.RegularExpressions;

namespace WeSafe.Services.Extensions
{
    public static class ValidateStringExtensions
    {
        public static bool IsValidPhoneNumber(this string str)
        {
           return Regex.IsMatch(str, Consts.Consts.phoneNumberRegularString);
        }

        public static bool IsValidIp(this string str)
        {
            return Regex.IsMatch(str, Consts.Consts.ipRegularString);
        }

        public static bool IsValidMacAddress(this string str)
        {
            return Regex.IsMatch(str, Consts.Consts.macAddressRegularString);
        }
    }
}
