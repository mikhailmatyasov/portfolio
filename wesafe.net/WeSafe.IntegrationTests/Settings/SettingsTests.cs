using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Account;
using WeSafe.IntegrationTests.Base;
using WeSafe.IntegrationTests.Client;
using WeSafe.IntegrationTests.Devices;
using WeSafe.Services.Client.Models;
using WeSafe.Shared.Extensions;
using Xunit;

namespace WeSafe.IntegrationTests.Settings
{
    public class SettingsTests : BaseTest
    {
        #region Fields

        private const string _baseUrl = "api/";
        private const string _validIp = "121.23.123.12";

        protected readonly ClientBaseTests clientBaseInstanse;
        protected readonly DevicesBaseTests deviceBaseInstanse;
        protected readonly HttpClient httpClient;
        protected readonly AccountTests accountsInstanse;

        #endregion

        #region Ctor

        public SettingsTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            deviceBaseInstanse = new DevicesBaseTests(factory);
            httpClient = _factory.CreateClient();
            clientBaseInstanse = new ClientBaseTests(factory);
            accountsInstanse = new AccountTests(factory);
        }

        #endregion

        #region Tests

        [Fact]
        public async Task GetDeviceSettings_DeviceSettingsCanBeRecievedViaMacAddress_AppropriateDeviceSettingsAreRecieved()
        {
            DeviceModel createdDevice = await deviceBaseInstanse.GetCreatedDeviceAsync();
            string url = _baseUrl + "device" + GetQueryUrlPart(createdDevice.MACAddress);

            await LoginDevice(createdDevice.MACAddress, createdDevice.Token);
            var result = await httpClient.GetAsync(url);

            var content = await result.Content.ReadAsStringAsync();

            WeSafe.Web.Core.Models.DeviceSettingsModel recivedDeviceSettings = await GetParsedResponseAsync<WeSafe.Web.Core.Models.DeviceSettingsModel>(result);
            Assert.Equal(createdDevice.SWVersion, recivedDeviceSettings.SwVersion);
            Assert.Equal(createdDevice.HWVersion, recivedDeviceSettings.HwVersion);
            Assert.Equal(createdDevice.CurrentSshPassword.Decrypt(), recivedDeviceSettings.SshPassword);
        }

        [Theory]
        [InlineData(null, _validIp)]
        [InlineData("", _validIp)]
        [InlineData("dffgdf2543534", _validIp)]
        [InlineData("00-14-22-04-25-37", _validIp)]
        [InlineData("00.21.22.bb.25.37", _validIp)]
        [InlineData("00:e0:4c:68:00:fa", null)]
        [InlineData("01:e0:4c:68:00:fa", "")]
        [InlineData("02:e0:4c:68:00:fa", "12.215")]
        [InlineData("03:e0:4c:68:00:fa", "fsdf.ss.dsf.g")]
        [InlineData("04:e0:4c:68:00:fa", _validIp)]
        public async Task GetDeviceSettings_DeviceSettingsCanNotBeRecievedViaInvalidMacAddressOrInvalidIpBox_IsNotSuccessfulStatusCode(string invalidMac, string invalidIp)
        {
            DeviceModel createdDevice = await deviceBaseInstanse.GetCreatedDeviceAsync();

            await LoginDevice(createdDevice.MACAddress, createdDevice.Token);

            string url = _baseUrl + "device" + GetQueryUrlPart(invalidMac, invalidIp);

            var result = await httpClient.GetAsync(url);

            Assert.False(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetCameraSettings_CameraSettingsCanBeRecievedViaValidMacAddress_IsSuccessfulStatusCode()
        {
            HttpClient authHttpClient = _factory.CreateClient();
            await accountsInstanse.AuthorizeUserAsync(authHttpClient);
            CameraModel createdCamera = await clientBaseInstanse.AddDeviceCamera(authHttpClient, true);
            DeviceModel createdDevice = deviceBaseInstanse.GetAllDevices().Result.FirstOrDefault(d => d.Id == createdCamera.DeviceId);
            string url = _baseUrl + "settings" + GetQueryUrlPart(createdDevice.MACAddress);

            await LoginDevice(createdDevice.MACAddress, createdDevice.Token);

            var result = await httpClient.GetAsync(url);

            Assert.True(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("dffgdf2543534")]
        [InlineData("00-14-22-04-25-37")]
        [InlineData("00.21.22.bb.25.37")]
        public async Task GetCameraSettings_CameraSettingsCanNotBeRecievedViaInvalidMacAddress_IsNotSuccessfulStatusCode(string invalidMac)
        {
            DeviceModel createdDevice = await deviceBaseInstanse.GetCreatedDeviceAsync();

            await LoginDevice(createdDevice.MACAddress, createdDevice.Token);

            string url = _baseUrl + "settings" + GetQueryUrlPart(invalidMac);

            var result = await httpClient.GetAsync(url);

            Assert.False(result.IsSuccessStatusCode);
        }

        #endregion

        #region PrivateMethods

        private string GetQueryUrlPart(string mac, string ip_box = null)
        {
            if (ip_box == null)
                return "?mac=" + mac;

            return "?mac=" + mac + "&ip_box=" + ip_box;
        }

        private async Task LoginDevice(string mac, string token)
        {
            var model = new DeviceAuthModel
            {
                Device = mac,
                Secret = token
            };
            var result = await httpClient.PostAsync("/api/device/auth",
                new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
            TokenResponse tokenResponse = await GetParsedResponseAsync<TokenResponse>(result);

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenResponse.AccessToken);
        }

        #endregion
    }
}
