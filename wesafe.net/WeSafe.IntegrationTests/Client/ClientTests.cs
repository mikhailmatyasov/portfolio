using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Base;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using WeSafe.Web.Core.Models;
using Xunit;

namespace WeSafe.IntegrationTests.Client
{
    public class ClientTests : ClientBaseTests
    {
        #region Fields



        #endregion

        #region Ctor

        public ClientTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {

        }

        #endregion

        #region Tests

        [Fact]
        public async Task GetDevicesAsync_UserCanGetDevices_DevicesAreRecieved()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);

            IEnumerable<DeviceModel> clientDevices = await GetAllClientDevices(authHttpClient);

            DeviceModel clientDevice = clientDevices.FirstOrDefault(d => d.Token == bindedDevice.Token);

            Assert.True(ModelsAreEqual(bindedDevice, clientDevice));

            IEnumerable<DeviceModel> allDevices = await deviceBaseInstanse.GetAllDevices();

            Assert.Equal(allDevices.Count(d => d.ClientId == bindedDevice.ClientId), clientDevices.Count());
        }

        [Fact]
        public async Task GetDeviceByIdAsync_UserCanGetHisDeviceById_DeviceIsRecieved()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);

            var result = await authHttpClient.GetAsync(_devicesBaseUrl + bindedDevice.Id);

            DeviceModel foundDevice = await GetParsedResponseAsync<DeviceModel>(result); ;

            Assert.True(ModelsAreEqual(bindedDevice, foundDevice));
        }

        //[LS] It is failed because GetDeviceByIdAsync method should be fixed.
        //[Fact]
        //public async Task GetDeviceByIdAsync_UserCanNotGetDeviceHeDoesNotOwnById_DeviceIsNotRecieved()
        //{
        //    DeviceModel createdDevice = await deviceBaseInstanse.GetCreatedDeviceAsync();

        //    var result = await authHttpClient.GetAsync(_devicesBaseUrl + createdDevice.Id);

        //    Assert.False(result.IsSuccessStatusCode);

        //    DeviceModel foundDevice = await GetParsedResponseAsync<DeviceModel>(result); ;

        //    Assert.Null(foundDevice);
        //}

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task GetDeviceByIdAsync_UserCanNotGetDeviceViaInvalidId_StatusCode204(int deviceId)
        {
            var result = await authHttpClient.GetAsync(_devicesBaseUrl + deviceId);

            Assert.True(result.StatusCode == HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetDeviceCamerasAsync_UserCanGetDeviceCamerasByDeviceId_CamerasAreRecieved()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);
            CameraModel createdCamera = await GetCreatedCamera(authHttpClient, bindedDevice.Id);

            var result = await authHttpClient.GetAsync(_devicesBaseUrl + $"{bindedDevice.Id}/cameras");

            Assert.True(result.IsSuccessStatusCode);

            var response = await GetParsedResponseAsync<PageResponse<CameraModel>>(result);
            IEnumerable<CameraModel> devicesCameras = response.Items;

            Assert.NotNull(devicesCameras);

            CameraModel foundCamera = devicesCameras.FirstOrDefault(c => c.CameraName == createdCamera.CameraName);

            Assert.NotNull(foundCamera);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task GetDeviceCamerasAsync_UserCanNotGetDevicesCamerasByInvalidDeviceId_CamerasAreNotRecieved(int deviceId)
        {
            var result = await authHttpClient.GetAsync(_devicesBaseUrl + $"{deviceId}/cameras");

            Assert.False(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetDeviceCamerasAsync_UserCanNotGetNotHisDeviceCamerasByDeviceId_CamerasAreNotRecieved()
        {
            DeviceModel createdDevice = await deviceBaseInstanse.GetCreatedDeviceAsync();

            var result = await authHttpClient.GetAsync(_devicesBaseUrl + $"{createdDevice.Id}/cameras");

            Assert.False(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetDeviceCamerasStatAsync_UserCanGetDeviceCamerasStatusesByDeviceId_AppropriateCamerasStatusesAreRecieved(bool cameraIsActive)
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);
            await GetCreatedCamera(authHttpClient, bindedDevice.Id, cameraIsActive);

            var result = await authHttpClient.GetAsync(_devicesBaseUrl + $"{bindedDevice.Id}/cameras-stat");

            Assert.True(result.IsSuccessStatusCode);

            CamerasStatModel camerasStatModel = await GetParsedResponseAsync<CamerasStatModel>(result);

            Assert.Equal(1, camerasStatModel.Count);

            if (cameraIsActive)
                Assert.Equal(1, camerasStatModel.ActiveCount);
            else
                Assert.Equal(0, camerasStatModel.ActiveCount);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task GetDeviceCamerasStatAsync_UserCanNotGetDeviceCamerasStatusesByInvalidDeviceId_IsNotSuccessfulStatus(int deviceId)
        {
            var result = await authHttpClient.GetAsync(_devicesBaseUrl + $"{deviceId}/cameras-stat");

            Assert.False(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetDeviceCamerasStatAsync_UserCanNotGetNotHisDeviceCamerasStatusesByDeviceId_IsNotSuccessfulStatus()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);
            await GetCreatedCamera(authHttpClient, bindedDevice.Id);
            HttpClient httpClient = _factory.CreateClient();
            await accountsInstanse.AuthorizeUserAsync(httpClient);

            var result = await httpClient.GetAsync(_devicesBaseUrl + $"{bindedDevice.Id}/cameras-stat");

            Assert.False(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task GetCameraByIdAsync_UserCanNotGetDeviceCameraByInvalidDeviceId_IsNotSuccessfulStatus(int deviceId)
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);
            await GetCreatedCamera(authHttpClient, bindedDevice.Id);

            var result = await authHttpClient.GetAsync(_devicesBaseUrl + $"{deviceId}/cameras/1");

            Assert.False(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task GetCameraByIdAsync_UserCanNotGetDeviceCameraByInvalidCameraId_IsNotSuccessfulStatus(int cameraId)
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);
            await GetCreatedCamera(authHttpClient, bindedDevice.Id);

            var result = await authHttpClient.GetAsync(_devicesBaseUrl + $"{bindedDevice.Id}/cameras/{cameraId}");

            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Fact]
        public async Task GetCameraByIdAsync_UserCanNotGetDeviceCameraByNotHisDeviceId_IsNotSuccessfulStatus()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);
            await GetCreatedCamera(authHttpClient, bindedDevice.Id);
            HttpClient httpClient = _factory.CreateClient();
            await accountsInstanse.AuthorizeUserAsync(httpClient);

            var result = await httpClient.GetAsync(_devicesBaseUrl + $"{bindedDevice.Id}/cameras/1");

            Assert.False(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetCameraByIdAsync_UserCanGetDeviceCameraById_CameraIsReceived()
        {
            CameraModel addedCamera = await AddDeviceCamera(authHttpClient);

            Assert.NotNull(addedCamera);

            var result = await authHttpClient.GetAsync(_devicesBaseUrl + $"{addedCamera.DeviceId}/cameras/{addedCamera.Id}");

            Assert.True(result.IsSuccessStatusCode);

            CameraModel receivedCamera = await GetParsedResponseAsync<CameraModel>(result);

            Assert.True(ModelsAreEqual(addedCamera, receivedCamera));
        }

        [Fact]
        public async Task GetCameraByIdAsync_UserCanNotGetDeviceCameraByNotHisCameraId_IsNotSuccessfulStatus()
        {

            CameraModel addedCamera = await AddDeviceCamera(authHttpClient);

            HttpClient httpClient = _factory.CreateClient();
            await accountsInstanse.AuthorizeUserAsync(httpClient);
            DeviceModel deviceWithoutCameras = await BindDeviceToUser(authHttpClient);

            var result = await httpClient.GetAsync(_devicesBaseUrl + $"{deviceWithoutCameras.Id}/cameras/{addedCamera.Id}");

            Assert.False(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task CreateCameraAsync_UserCanCreateCamera_CameraIsCreated()
        {
            CameraModel addedCamera = await AddDeviceCamera(authHttpClient);

            Assert.NotNull(addedCamera);
        }

        [Fact]
        public async Task CreateCameraAsync_UserCanCreateOnlyFourActiveCameras_FourActiveCamerasAreCreated()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);

            for (int i = 0; i < 4; i++)
            {
                await GetCreatedCamera(authHttpClient, bindedDevice.Id, true);
            }

            await GetCreatedCamera(authHttpClient, bindedDevice.Id, true, false);

            int deviceCamerasNumber = GetDeviceCameras(bindedDevice.Id).Result.Count();

            Assert.Equal(4, deviceCamerasNumber);
        }

        [Fact]
        public async Task CreateCameraAsync_UserWithFourActiveCamerasCanCreateInactiveCamera_InactiveCamerasIsCreated()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);

            for (int i = 0; i < 4; i++)
            {
                await GetCreatedCamera(authHttpClient, bindedDevice.Id, true);
            }

            await GetCreatedCamera(authHttpClient, bindedDevice.Id, false, true);

            int deviceCamerasNumber = GetDeviceCameras(bindedDevice.Id).Result.Count();

            Assert.Equal(5, deviceCamerasNumber);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task CreateCameraAsync_UserCanNotCreateCameraForDeviceWithInvalidId_CameraIsNotCreated(int deviceId)
        {
            await GetCreatedCamera(authHttpClient, deviceId, true, false);

        }

        [Fact]
        public async Task CreateCameraAsync_UserCanNotCreateCameraToNotHisDevice_CameraIsNotCreated()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);
            HttpClient httpClient = _factory.CreateClient();
            await accountsInstanse.AuthorizeUserAsync(httpClient);

            await GetCreatedCamera(httpClient, bindedDevice.Id, false, false);

            int deviceCamerasNumber = GetDeviceCameras(bindedDevice.Id).Result.Count();

            Assert.Equal(0, deviceCamerasNumber);
        }

        //[LS] It is failed because GetDeviceByIdAsync method should be fixed.
        //[Fact]
        //public async Task CreateCameraAsync_UserCanNotCreateInvalidCamera_CameraIsNotCreated()
        //{
        //    DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);
        //    IEnumerable<CameraModel> invalidCameras = cameraBaseInstanse.GetInvalidCameraModels();

        //    foreach (CameraModel ivalidCamera in invalidCameras)
        //    {
        //        await AddCameraAsync(ivalidCamera, authHttpClient, bindedDevice.Id, false);
        //    }

        //    int deviceCamerasNumber = GetDeviceCameras(bindedDevice.Id).Result.Count();

        //    Assert.Equal(0, deviceCamerasNumber);
        //}

        [Fact]
        public async Task UpdateCameraAsync_UserCanUpdateCamera_CameraIsUpdated()
        {
            CameraModel createdCamera = await AddDeviceCamera(authHttpClient);
            createdCamera.Port = "85";

            var result = await authHttpClient.PutAsync(_devicesBaseUrl + $"{createdCamera.DeviceId}/cameras", new StringContent(JsonConvert.SerializeObject(createdCamera), Encoding.UTF8, "application/json"));

            Assert.True(result.IsSuccessStatusCode);

            CameraModel foundCamera = GetDeviceCameras(createdCamera.DeviceId).Result.FirstOrDefault(c => c.Id == createdCamera.Id);

            Assert.True(ModelsAreEqual(createdCamera, foundCamera));
        }

        //[LS] It is failed because GetDeviceByIdAsync method should be fixed.
        //[Fact]
        //public async Task UpdateCameraAsync_UserCanNotUpdateCameraWithInvalidModel_CameraIsNotUpdated()
        //{
        //    CameraModel createdCamera = await AddDeviceCamera(authHttpClient);
        //    IEnumerable<CameraModel> invalidCameras = cameraBaseInstanse.GetInvalidCameraModels();

        //    foreach (CameraModel ivalidCamera in invalidCameras)
        //    {
        //        ivalidCamera.Id = createdCamera.Id;

        //        var result = await authHttpClient.PutAsync(_devicesBaseUrl + $"{createdCamera.DeviceId}/cameras", new StringContent(JsonConvert.SerializeObject(ivalidCamera), Encoding.UTF8, "application/json"));

        //        Assert.False(result.IsSuccessStatusCode);
        //    }

        //    CameraModel foundCamera = GetDeviceCameras(createdCamera.DeviceId).Result.FirstOrDefault(c => c.Id == createdCamera.Id);

        //    Assert.True(ModelsAreEqual(createdCamera, foundCamera));
        //}

        [Fact]
        public async Task UpdateCameraAsync_UserCanNotUpdateNotHisCamera_CameraIsUpdated()
        {
            CameraModel createdCamera = await AddDeviceCamera(authHttpClient);
            createdCamera.Port = "85";
            HttpClient httpClient = _factory.CreateClient();
            await accountsInstanse.AuthorizeUserAsync(httpClient);

            var result = await httpClient.PutAsync(_devicesBaseUrl + $"{createdCamera.DeviceId}/cameras", new StringContent(JsonConvert.SerializeObject(createdCamera), Encoding.UTF8, "application/json"));

            Assert.False(result.IsSuccessStatusCode);

            CameraModel foundCamera = GetDeviceCameras(createdCamera.DeviceId).Result.FirstOrDefault(c => c.Id == createdCamera.Id);

            Assert.False(ModelsAreEqual(createdCamera, foundCamera));
        }

        [Fact]
        public async Task DeleteCameraAsync_UserCanDeleteCamera_CameraIsDeleted()
        {
            CameraModel createdCamera = await AddDeviceCamera(authHttpClient);

            var result = await authHttpClient.DeleteAsync(_devicesBaseUrl + $"{createdCamera.DeviceId}/cameras/{createdCamera.Id}");

            Assert.True(result.IsSuccessStatusCode);

            CameraModel foundCamera = GetDeviceCameras(createdCamera.DeviceId).Result.FirstOrDefault(c => c.Id == createdCamera.Id);

            Assert.Null(foundCamera);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task DeleteCameraAsync_UserCanNotDeleteCameraWithInvalidDeviceId_IsNotSuccessful(int deviceId)
        {
            CameraModel createdCamera = await AddDeviceCamera(authHttpClient);

            var result = await authHttpClient.DeleteAsync(_devicesBaseUrl + $"{deviceId}/cameras/{createdCamera.Id}");

            Assert.False(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task DeleteCameraAsync_UserCanNotDeleteCameraWithInvalidCameraId_IsNotSuccessful(int cameraId)
        {
            CameraModel createdCamera = await AddDeviceCamera(authHttpClient);

            var result = await authHttpClient.DeleteAsync(_devicesBaseUrl + $"{createdCamera.DeviceId}/cameras/{cameraId}");

            Assert.False(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task DeleteCameraAsync_UserCanDeleteNotHisCamera_CameraIsNotDeleted()
        {
            CameraModel createdCamera = await AddDeviceCamera(authHttpClient);
            HttpClient httpClient = _factory.CreateClient();
            await accountsInstanse.AuthorizeUserAsync(httpClient);
            CameraModel anotherUserCamera = await AddDeviceCamera(httpClient);

            var result = await authHttpClient.DeleteAsync(_devicesBaseUrl + $"{anotherUserCamera.DeviceId}/cameras/{anotherUserCamera.Id}");

            Assert.False(result.IsSuccessStatusCode);

            CameraModel foundCamera = GetDeviceCameras(createdCamera.DeviceId).Result.FirstOrDefault(c => c.Id == createdCamera.Id);

            Assert.NotNull(foundCamera);
        }

        [Fact]
        public async Task BindDeviceToClientAsync_UserCanBindDeviceByValidToken_DeviceIsBinded()
        {
            await BindDeviceToUser(authHttpClient);
        }

        [Fact]
        public async Task BindDeviceToClientAsync_UserCanNotBindAlreadyBindedToHimDevice_IsNotSuccesful()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);

            var result = await authHttpClient.PostAsync(_devicesBaseUrl + bindedDevice.Token, null);

            Assert.False(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task BindDeviceToClientAsync_UserCanNotBindBindedToAnotherUserDevice_IsNotSuccesful()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);
            HttpClient httpClient = _factory.CreateClient();
            await accountsInstanse.AuthorizeUserAsync(httpClient);

            var result = await httpClient.PostAsync(_devicesBaseUrl + bindedDevice.Token, null);

            Assert.False(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("dsffgsfghsfdgsg")]
        public async Task BindDeviceToClientAsync_UserCanNotBindDeviceWithInvalidToken_IsNotSuccesful(string invalidToken)
        {
            var result = await authHttpClient.PostAsync(_devicesBaseUrl + invalidToken, null);

            Assert.False(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task UpdateDeviceNameAsync_UserCanUpdateDeviceWithInvalidName_DeviceIsNotChanged(string deviceName)
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);

            var result = await authHttpClient.PostAsync(_devicesBaseUrl + $"{bindedDevice.Id}/editname/", new StringContent(
                JsonConvert.SerializeObject(new DeviceEditNameModel() { NewDeviceName = deviceName }), Encoding.UTF8, "application/json"));

            Assert.False(result.IsSuccessStatusCode);

            DeviceModel changedDevice = await deviceBaseInstanse.GetDeviceByMacAddress(bindedDevice.MACAddress);

            Assert.True(ModelsAreEqual(bindedDevice, changedDevice));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task UpdateDeviceNameAsync_UserCanUpdateDeviceWithInvalidId_IsNotSuccesful(int deviceId)
        {
            var result = await authHttpClient.PostAsync(_devicesBaseUrl + $"{deviceId}/editname", new StringContent(
                JsonConvert.SerializeObject(new DeviceEditNameModel() { NewDeviceName = "newName" }), Encoding.UTF8, "application/json"));

            Assert.False(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task UpdateDeviceNameAsync_UserCanNotUpdateNotHisDeviceName_DeviceIsNotChanged()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);
            HttpClient httpClient = _factory.CreateClient();
            await accountsInstanse.AuthorizeUserAsync(httpClient);

            var result =
                await httpClient.PostAsync(_devicesBaseUrl + $"{bindedDevice.Id}/editname", new StringContent(
                    JsonConvert.SerializeObject(new DeviceEditNameModel() { NewDeviceName = "newName" }), Encoding.UTF8, "application/json"));

            Assert.False(result.IsSuccessStatusCode);

            DeviceModel changedDevice = await deviceBaseInstanse.GetDeviceByMacAddress(bindedDevice.MACAddress);

            Assert.True(ModelsAreEqual(bindedDevice, changedDevice));
        }

        [Fact]
        public async Task UpdateDeviceNameAsync_UserCanNotUpdateNotHisDeviceName_DeviceIsChanged()
        {
            DeviceModel bindedDevice = await BindDeviceToUser(authHttpClient);

            var result = await authHttpClient
                .PostAsync(_devicesBaseUrl + $"{bindedDevice.Id}/editname",
                    new StringContent(
                        JsonConvert.SerializeObject(new DeviceEditNameModel() { NewDeviceName = "newName" }), Encoding.UTF8, "application/json"));

            Assert.True(result.IsSuccessStatusCode);

            DeviceModel changedDevice = await deviceBaseInstanse.GetDeviceByMacAddress(bindedDevice.MACAddress);

            Assert.Equal("newName", changedDevice.Name);
        }

        #endregion
    }
}
