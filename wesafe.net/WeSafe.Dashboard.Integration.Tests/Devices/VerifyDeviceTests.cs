using System.Net.Http;
using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests;
using WeSafe.Dashboard.WebApi;
using WeSafe.Dashboard.WebApi.Commands.Devices;
using WeSafe.Dashboard.WebApi.Enumerations;
using WeSafe.Dashboard.WebApi.Models;
using Xunit;

namespace WeSafe.Dashboard.Integration.Tests.Devices
{
    public class VerifyDeviceTests : DashboardBaseTest, IClassFixture<DashboardWebApplicationFactory<Startup>>,
        IClassFixture<AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup>>
    {
        public VerifyDeviceTests(DashboardWebApplicationFactory<Startup> factory,
            AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup> authFactory) : base(factory, authFactory)
        {
        }

        [Fact]
        public async Task VerifyAttachedDevice_ReturnsAttachedResult()
        {
            var result = await Post<VerifyDeviceResult>("/api/verify-device", new VerifyDeviceByTokenCommand
            {
                DeviceToken = "723456abc"
            });

            Assert.NotNull(result);
            Assert.Equal(DeviceVerificationStatus.Attached, result.Status);
        }

        [Fact]
        public async Task VerifyNonExistingDevice_ReturnsNotFoundResult()
        {
            var result = await Post<VerifyDeviceResult>("/api/verify-device", new VerifyDeviceByTokenCommand
            {
                DeviceToken = "65785dffgf"
            });

            Assert.NotNull(result);
            Assert.Equal(DeviceVerificationStatus.NotFound, result.Status);
        }
    }
}