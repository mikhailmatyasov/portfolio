using System;
using WeSafe.Shared.Extensions;

namespace WeSafe.Shared.Services
{
    public class UniqueStringService
    {
        public static string GenerateUniqueString(byte length)
        {
            var password = Guid.NewGuid().ToString("N").Substring(0, length).ToUpper();

            if (password.IsContainsDigitAndLetter())
                return password;

            return GenerateUniqueString(length);
        }
    }
}
