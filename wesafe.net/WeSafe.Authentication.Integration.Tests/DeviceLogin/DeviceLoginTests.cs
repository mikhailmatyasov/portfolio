using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WeSafe.Authentication.Integration.Tests.Static;
using WeSafe.Authentication.WebApi;
using WeSafe.Authentication.WebApi.Commands.DeviceLogin;
using WeSafe.DAL;
using WeSafe.Web.Common.Authentication;
using WeSafe.Web.Common.Exceptions.Models;
using Xunit;

namespace WeSafe.Authentication.Integration.Tests.DeviceLogin
{
    public class DeviceLoginTests : BaseTest<Startup>,
        IClassFixture<AuthenticationWebApplicationFactory<Startup>>
    {
        private AuthenticationWebApplicationFactory<Startup> _factory;

        public DeviceLoginTests(AuthenticationWebApplicationFactory<Startup> factory) : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task DeviceLogin_LoginByDevice_Success()
        {
            var result = await Post<TokenResponseModel>(AuthenticationUrls.deviceLoginUrl, new DeviceLoginCommand
            {
                MacAddress = "00:30:48:5a:58:65",
                Secret = null
            });

            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.UserName);
            Assert.True(result.Role == Shared.Roles.UserRoles.Devices);

            CleanupDevice();
        }

        [Theory]
        [InlineData("10:30:48:5a:58:65", null)]
        [InlineData("00:30:48:5a:58:65", "123457")]
        public async Task DeviceLogin_WrongDeviceOrSecret(string device, string secret)
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.deviceLoginUrl, new DeviceLoginCommand
            {
                MacAddress = device,
                Secret = secret
            });

            Assert.NotNull(result);
            Assert.True(result.Code == 401);
        }

        [Theory]
        [InlineData("")]
        [InlineData("00:30:48:5T:58:65")]
        [InlineData(null)]
        public async Task DeviceLogin_InvalidDevice(string device)
        {
            var result = await Post<ErrorModel>(AuthenticationUrls.deviceLoginUrl, new DeviceLoginCommand
            {
                MacAddress = device,
                Secret = null
            });

            Assert.NotNull(result);
            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task DeviceLogin_LoginAfterPreviousLoginWithSecret_Success()
        {
            var result = await Post<TokenResponseModel>(AuthenticationUrls.deviceLoginUrl, new DeviceLoginCommand
            {
                MacAddress = "00:30:48:5a:58:65",
                Secret = null
            });

            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);

            result = await Post<TokenResponseModel>(AuthenticationUrls.deviceLoginUrl, new DeviceLoginCommand
            {
                MacAddress = "00:30:48:5a:58:65",
                Secret = result.AccessToken
            });

            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);

            CleanupDevice();
        }

        [Fact]
        public async Task DeviceLogin_LoginAfterPreviousLoginWithInvalidSecret()
        {
            var result = await Post<TokenResponseModel>(AuthenticationUrls.deviceLoginUrl, new DeviceLoginCommand
            {
                MacAddress = "00:30:48:5a:58:65",
                Secret = null
            });

            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);

            var error = await Post<ErrorModel>(AuthenticationUrls.deviceLoginUrl, new DeviceLoginCommand
            {
                MacAddress = "00:30:48:5a:58:65",
                Secret = "some_invalid_value"
            });

            Assert.NotNull(error);
            Assert.True(error.Code == 401);

            CleanupDevice();
        }

        private void CleanupDevice()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<WeSafeDbContext>();
            var device = context.Devices.FirstOrDefault();

            if ( device != null )
            {
                device.AuthToken = null;
                context.SaveChanges();
            }
        }
    }
}