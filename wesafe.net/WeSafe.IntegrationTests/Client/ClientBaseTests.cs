using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Account;
using WeSafe.IntegrationTests.Base;
using WeSafe.IntegrationTests.Cameras;
using WeSafe.IntegrationTests.Devices;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using Xunit;

namespace WeSafe.IntegrationTests.Client
{
    public class ClientBaseTests : BaseTest
    {
        #region Fields

        protected readonly AccountTests accountsInstanse;
        protected readonly DevicesBaseTests deviceBaseInstanse;
        protected readonly CameraBaseTests cameraBaseInstanse;

        protected const string _baseUrl = "/api/client/";
        protected const string _devicesBaseUrl = _baseUrl + "devices/";
        protected readonly HttpClient authHttpClient;

        #endregion

        #region Ctor

        public ClientBaseTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            deviceBaseInstanse = new DevicesBaseTests(factory);
            accountsInstanse = new AccountTests(factory);
            cameraBaseInstanse = new CameraBaseTests(factory);
            authHttpClient = factory.CreateClient();
            accountsInstanse.AuthorizeUserAsync(authHttpClient).Wait();
        }

        #endregion

        #region Public Methods

        public async Task<CameraModel> AddDeviceCamera(HttpClient httpClient, bool isActiveCamera = false)
        {
            DeviceModel bindedDevice = await BindDeviceToUser(httpClient);
            await GetCreatedCamera(httpClient, bindedDevice.Id, isActiveCamera);

            var result = await httpClient.GetAsync(_devicesBaseUrl + $"{bindedDevice.Id}/cameras");

            Assert.True(result.IsSuccessStatusCode);

            return GetParsedResponseAsync<PageResponse<CameraModel>>(result).Result.Items.FirstOrDefault();
        }

        #endregion

        #region Protected 

        protected async Task<IEnumerable<DeviceModel>> GetAllClientDevices(HttpClient httpClient)
        {
            var result = await httpClient.GetAsync(_baseUrl + "devices");

            Assert.True(result.IsSuccessStatusCode);

            IEnumerable<DeviceModel> clientDeices = await GetParsedResponseAsync<IEnumerable<DeviceModel>>(result);

            Assert.NotNull(clientDeices);

            return clientDeices;
        }

        protected async Task<DeviceModel> BindDeviceToUser(HttpClient httpClient)
        {
            DeviceModel createdDevice = await deviceBaseInstanse.GetCreatedDeviceAsync();

            var result = await httpClient.PostAsync(_baseUrl + "devices/" + createdDevice.Token, null);

            Assert.True(result.IsSuccessStatusCode);

            DeviceModel bindedDevice = await deviceBaseInstanse.GetDeviceByMacAddress(createdDevice.MACAddress);

            Assert.NotNull(bindedDevice);

            return bindedDevice;
        }

        protected async Task<CameraModel> GetCreatedCamera(HttpClient httpClient, int deviceId, bool cameraIsActive = false, bool isSucceeded = true)
        {
            CameraModel validCameraModel = cameraBaseInstanse.GetValidCameraModel(cameraIsActive);
            await AddCameraAsync(validCameraModel, httpClient, deviceId, isSucceeded);

            if (!isSucceeded)
                return null;

            validCameraModel.DeviceId = deviceId;

            return validCameraModel;
        }

        protected async Task AddCameraAsync(CameraModel cameraModel, HttpClient httpClient, int deviceId, bool isSucceeded)
        {
            var result = await httpClient.PostAsync(_baseUrl + $"devices/{deviceId}/cameras", new StringContent(JsonConvert.SerializeObject(cameraModel), Encoding.UTF8, "application/json"));

            if (isSucceeded)
                Assert.True(result.IsSuccessStatusCode);
            else
                Assert.False(result.IsSuccessStatusCode);
        }

        protected async Task<IEnumerable<CameraModel>> GetDeviceCameras(int deviceId)
        {
            var result = await authHttpClient.GetAsync(_devicesBaseUrl + $"{deviceId}/cameras");

            Assert.True(result.IsSuccessStatusCode);

            var response = await GetParsedResponseAsync<PageResponse<CameraModel>>(result);

            return response.Items;
        }

        #endregion
    }
}
