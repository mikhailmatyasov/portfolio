using WeSafe.Shared.Extensions;
using WeSafe.Shared.Services;
using Xunit;

namespace WeSafe.UnitTests.PasswordTests
{
    public class UniqueStringServiceTests
    {
        private byte _passwordLength = 8;


        [Fact]
        public void GenerateUniqueString_StringContainLettersAndNumbers_StringHaveAtLeastOneLetterAndOneNumber()
        {
            var password = UniqueStringService.GenerateUniqueString(_passwordLength);

            Assert.True(password.IsContainsDigitAndLetter());
            Assert.Equal(_passwordLength, password.Length);
        }
    }
}
