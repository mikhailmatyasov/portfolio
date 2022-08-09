using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests;
using WeSafe.Dashboard.WebApi;
using WeSafe.Dashboard.WebApi.Models;
using WeSafe.Web.Common.Exceptions.Models;
using Xunit;

namespace WeSafe.Dashboard.Integration.Tests.Devices
{
    public class DeviceTests : DashboardBaseTest, IClassFixture<DashboardWebApplicationFactory<Startup>>,
        IClassFixture<AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup>>
    {
        public DeviceTests(DashboardWebApplicationFactory<Startup> factory,
            AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup> authFactory) : base(factory, authFactory)
        {
        }

        [Fact]
        public async Task GetClientDevicesAsync_ProcessSuccess()
        {
            Authorize("user1").GetAwaiter().GetResult();

            var result = await Get<IEnumerable<DeviceModel>>("/api/client/devices");

            Assert.True(result.Any());

            var device = result.First();

            Assert.Equal("Device1", device.Name);
            Assert.Equal("1a:30:48:5a:58:65", device.MACAddress);
            Assert.Equal("723456abc", device.Token);
        }

        [Fact]
        public async Task GetClientDeviceAsync_ProcessSuccess()
        {
            Authorize("user1").GetAwaiter().GetResult();

            var result = await Get<IEnumerable<DeviceModel>>("/api/client/devices");

            Assert.True(result.Any());

            var device = await Get<DeviceModel>($"/api/client/devices/{result.First().Id}");

            Assert.NotNull(device);
        }

        [Theory]
        [InlineData(-3)]
        [InlineData(0)]
        public async Task GetClientDeviceAsync_InvalidDeviceId(int deviceId)
        {
            Authorize("user1").GetAwaiter().GetResult();

            var result = await Get<ErrorModel>($"/api/client/devices/{deviceId}");

            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task GetClientDeviceAsync_InvalidDeviceNotFound()
        {
            Authorize("user1").GetAwaiter().GetResult();

            var result = await Get<ErrorModel>($"/api/client/devices/45656");

            Assert.True(result.Code == 500);
        }

        [Theory]
        [InlineData("234455")]
        [InlineData("dfdsfsdfds")]
        public async Task AttachDeviceToClient_InvalidDeviceToken(string token)
        {
            Authorize("user1").GetAwaiter().GetResult();

            var result = await Post<ErrorModel>($"/api/client/devices/{token}", null);

            Assert.True(result.Code == 400);
        }

        [Fact]
        public async Task AttachDeviceToClient_DeviceTokenNotFound()
        {
            Authorize("user1").GetAwaiter().GetResult();

            var result = await Post<ErrorModel>($"/api/client/devices/65785dffgf", null);

            Assert.True(result.Code == 500);
        }

        [Fact]
        public async Task AttachDeviceToClient_DeviceTokenAlreadyAttached()
        {
            Authorize("user1").GetAwaiter().GetResult();

            var result = await Post<ErrorModel>($"/api/client/devices/723456abc", null);

            Assert.True(result.Code == 500);
        }

        [Fact]
        public async Task AttachDeviceToClient_ProcessSuccess()
        {
            Authorize("user1").GetAwaiter().GetResult();

            await Post($"/api/client/devices/123456abc", null);
        }
    }
}