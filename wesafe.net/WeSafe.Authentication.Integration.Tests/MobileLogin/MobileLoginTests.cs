using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests.Static;
using WeSafe.Authentication.WebApi;
using WeSafe.Authentication.WebApi.Commands.MobileLogin;
using WeSafe.Web.Common.Authentication;
using WeSafe.Web.Common.Exceptions.Models;
using Xunit;

namespace WeSafe.Authentication.Integration.Tests.MobileLogin
{
    public class MobileLoginTests : BaseTest<Startup>,
        IClassFixture<AuthenticationWebApplicationFactory<Startup>>
    {
        private AuthenticationWebApplicationFactory<Startup> _factory;

        public MobileLoginTests(AuthenticationWebApplicationFactory<Startup> factory) : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task MobileLogin_LoginByPhone_Success()
        {
            var result = await Post<TokenResponseModel>(AuthenticationUrls.mobileLoginUrl, new MobileLoginCommand
            {
                PhoneNumber = "+71234567890"
            });

            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.UserName);
            Assert.True(result.Role == Shared.Roles.UserRoles.Users);
        }

        [Fact]
        public async Task MobileLogin_WrongPhone()
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.mobileLoginUrl, new MobileLoginCommand
            {
                PhoneNumber = "+71234567891"
            });

            Assert.NotNull(result);
            Assert.True(result.Code == 401);
        }

        [Theory]
        [InlineData("")]
        [InlineData("number")]
        [InlineData("+73545677678756")]
        [InlineData(null)]
        public async Task MobileLogin_InvalidPhone(string phone)
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.deviceLoginUrl, new MobileLoginCommand
            {
                PhoneNumber = phone
            });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }
    }
}