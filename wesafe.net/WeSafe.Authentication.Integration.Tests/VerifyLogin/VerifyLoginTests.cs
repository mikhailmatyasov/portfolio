using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests.Static;
using WeSafe.Authentication.WebApi;
using WeSafe.Authentication.WebApi.Commands.VerifyLogin;
using WeSafe.Authentication.WebApi.Enumerations;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Web.Common.Exceptions.Models;
using Xunit;

namespace WeSafe.Authentication.Integration.Tests.VerifyLogin
{
    public class VerifyLoginTests : BaseTest<Startup>,
        IClassFixture<AuthenticationWebApplicationFactory<Startup>>
    {
        private AuthenticationWebApplicationFactory<Startup> _factory;

        public VerifyLoginTests(AuthenticationWebApplicationFactory<Startup> factory) : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task VerifyLogin_NotExists_Success()
        {
            var result = await Post<VerifyLoginResult>(AuthenticationUrls.verifyLoginUrl, new VerifyLoginCommand
            {
                UserName = "some_login"
            });

            Assert.NotNull(result);
            Assert.Equal(LoginStatus.Ok, result.Status);
        }

        [Fact]
        public async Task VerifyLogin_AlreadyExists_ReturnExists()
        {
            var result = await Post<VerifyLoginResult>(AuthenticationUrls.verifyLoginUrl, new VerifyLoginCommand
            {
                UserName = "user1"
            });

            Assert.NotNull(result);
            Assert.Equal(LoginStatus.Exists, result.Status);
        }

        [Fact]
        public async Task VerifyLogin_EmptyLogin_ReturnBadRequest()
        {
            var error = await Post<ErrorModel>(AuthenticationUrls.verifyLoginUrl, new VerifyLoginCommand
            {
                UserName = null
            });

            Assert.NotNull(error);
            Assert.True(error.Code == 400);
        }

        [Fact]
        public async Task VerifyLogin_LoginTooLong_ReturnBadRequest()
        {
            var error = await Post<ErrorModel>(AuthenticationUrls.verifyLoginUrl, new VerifyLoginCommand
            {
                UserName = new string('s', 51)
            });

            Assert.NotNull(error);
            Assert.True(error.Code == 400);
        }
    }
}