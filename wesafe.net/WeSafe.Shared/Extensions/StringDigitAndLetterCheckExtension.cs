using System.Linq;

namespace WeSafe.Shared.Extensions
{
    public static class StringDigitAndLetterCheckExtension
    {
        public static bool IsContainsDigitAndLetter(this string checkedString)
        {
            return checkedString.Any(char.IsDigit) && checkedString.Any(char.IsLetter);
        }
    }
}
