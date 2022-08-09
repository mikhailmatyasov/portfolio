using System;
using WeSafe.Authentication.WebApi.Models;
using Xunit;

namespace WeSafe.Authentication.Unit.Tests
{
    public class TokenRequestModelTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void TokenRequestModel_Ctor_InvalidIdentifier_ThrowsArgumentNullException(string identifier)
        {
            Assert.Throws<ArgumentNullException>(
                () => new TokenRequestModel(identifier, "Suoername", DateTime.MaxValue));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void TokenRequestModel_Ctor_InvalidName_ThrowsArgumentNullException(string name)
        {
            Assert.Throws<ArgumentNullException>(
                () => new TokenRequestModel("someIdentifier", name, DateTime.MaxValue));
        }
    }
}
