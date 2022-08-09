using System;
using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests.Static;
using WeSafe.Authentication.WebApi;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Web.Common.Authentication;
using WeSafe.Web.Common.Exceptions.Models;
using Xunit;

namespace WeSafe.Authentication.Integration.Tests.Login
{
    public class LoginTests : BaseTest<Startup>,
        IClassFixture<AuthenticationWebApplicationFactory<Startup>>
    {
        public LoginTests(AuthenticationWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Login_SuccessLoginByAdmin()
        {
            var result = await Post<TokenResponseModel>(AuthenticationUrls.loginUrl, new { Username = "Admin", Password = "123456" });

            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.DisplayName);
            Assert.True(result.Role == Shared.Roles.UserRoles.Administrators);
        }

        [Theory]
        [InlineData("admin1", "123456")]
        [InlineData("admin", "123457")]
        [InlineData("admin1", "123457")]
        public async Task Login_InvalidUsernameOrPassword(string username, string password)
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = username, Password = password });

            Assert.NotNull(result);
            Assert.True(result.Code == 401);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("    ")]
        public async Task Login_InvalidUsernameData(string username)
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = username, Password = "123456" });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("    ")]
        public async Task Login_InvalidPasswordData(string password)
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = "Admin", Password = password });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task Login_UsernameTooLong()
        {
            var username = new String('*', 51);

            var result = await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = username, Password = "123456" });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task Login_PasswordTooLong()
        {
            var password = new String('*', 53);

            var result = await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = "Admin", Password = password });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task Login_PasswordTooShort()
        {
            var password = new String('*', 3);

            var result = await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = "Admin", Password = password });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task Login_UserLockedAfter3InvalidAttempts()
        {
            await Login_SuccessLoginByUser();

            await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = "user", Password = "1234567" });
            await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = "user", Password = "1234567" });
            await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = "user", Password = "1234567" });

            var result = await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = "user", Password = "123456" });

            Assert.NotNull(result);
            Assert.True(result.Code == 401);
        }

        [Fact]
        public async Task Login_AdministratorNotLockedAfter3InvalidAttempts()
        {
            await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = "Admin", Password = "123" });
            await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = "Admin", Password = "123" });
            await Post<ErrorModel>(AuthenticationUrls.loginUrl, new { Username = "Admin", Password = "123" });

            await Login_SuccessLoginByAdmin();
        }

        private async Task Login_SuccessLoginByUser()
        {
            var result = await Post<TokenResponseModel>(AuthenticationUrls.loginUrl, new { Username = "user", Password = "123456" });

            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.DisplayName);
            Assert.True(result.Role == Shared.Roles.UserRoles.Users);
        }
    }
}
