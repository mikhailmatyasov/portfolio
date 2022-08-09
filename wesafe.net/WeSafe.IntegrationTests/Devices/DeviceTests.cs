using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Account;
using WeSafe.IntegrationTests.Base;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using Xunit;

namespace WeSafe.IntegrationTests.Devices
{
    public class DeviceTests : DevicesBaseTests
    {
        #region Fields

        private readonly AccountTests accountsInstanse;

        #endregion

        #region Ctor

        public DeviceTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            accountsInstanse = new AccountTests(factory);
        }

        #endregion

        #region Tests

        [Fact]
        public async Task GetDeviceByIdAsync_AdminCanGetDeviceById_AppropriateDeviceIsFound()
        {
            DeviceModel createdDevice = await GetCreatedDeviceAsync();

            var foundDevice = await GetAsync<DeviceModel>(_baseUrl + createdDevice.Id);

            Assert.True(ModelsAreEqual(createdDevice, foundDevice));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task GetDeviceByIdAsync_AdminCanNotGetDeviceByViaNotExistingId_StatusCode204(int deviceId)
        {
            var result = await AdminHttpClient.GetAsync(_baseUrl + deviceId);

            Assert.True(result.StatusCode == HttpStatusCode.NoContent);
        }

        [Theory]
        [InlineData("1111")]
        [InlineData("____")]
        [InlineData("*/43243Хсвы{}")]
        [InlineData("你好")]
        public async Task CreateDeviceAsync_AdminCanCreateValidDevice_IsSuccessfulStatusAsync(string name)
        {
            string macAddress = GenerateValidMacAddress();
            DeviceModel deviceModel = CreateDeviceModel(macAddress);
            deviceModel.Name = name;

            HttpResponseMessage result = await AddDeviceAsync(deviceModel);

            Assert.True(result.IsSuccessStatusCode);

            DeviceModel device = await GetDeviceByMacAddress(macAddress);

            Assert.True(device.MACAddress == macAddress);
        }

        [Theory]
        [InlineData("00:e0:4c", "Hello")]
        [InlineData("000a959d6816", "Hello")]
        [InlineData("00-0a-95-9d-68-16", "Hello")]
        [InlineData(null, "111")]
        [InlineData("", "111")]
        [InlineData("    ", "111")]
        [InlineData("00:0a:95:9d:68:16", "")]
        [InlineData("00:0a:95:9d:68:16", "    ")]
        [InlineData("00:0a:95:9d:68:16", null)]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("    ", "    ")]
        public async Task CreateDeviceAsync_InvalidModel_DeviceIsNotCreated(string macAddress, string name)
        {
            var newDevice = CreateDeviceModel(macAddress);
            newDevice.Name = name;

            HttpResponseMessage result = await AddDeviceAsync(newDevice);

            Assert.False(result.IsSuccessStatusCode);

            DeviceModel device = await GetDeviceByMacAddress(macAddress);

            Assert.True(device == null);
        }

        [Fact]
        public async Task CreateDeviceAsync_AdminCanNotCreateDeviceWithExistenceMacAddress_IsNotSuccessfulStatusAsync()
        {
            string macAddress = GenerateValidMacAddress();
            await AddValidDeviceAsync(macAddress);

            DeviceModel deviceModel = CreateDeviceModel(macAddress);

            HttpResponseMessage result = await AddDeviceAsync(deviceModel);

            Assert.False(result.IsSuccessStatusCode);

            IEnumerable<DeviceModel> devices = (await GetAllDevices()).Where(d => d.MACAddress == macAddress);

            Assert.True(devices.Count() == 1);
        }

        [Fact]
        public async Task CreateDeviceAsync_CanNotAddNullDevice_IsNotSuccessfulStatusAsync()
        {
            HttpResponseMessage result = await AddDeviceAsync(null);

            Assert.False(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("111", "newNvidiaSv", true)]
        [InlineData(".....", "__432", false)]
        [InlineData("$$^^&&", "$$^^**", true)]
        [InlineData("___", ")))", true)]
        [InlineData("你好", "你好", true)]
        public async Task UpdateDeviceAsync_ValidModelParameters_DeviceIsUpdated(string name, string NVIDIASn, bool isArmed)
        {
            DeviceModel createdDevice = await GetCreatedDeviceAsync();
            createdDevice.Name = name;
            createdDevice.NVIDIASn = NVIDIASn;
            createdDevice.IsArmed = isArmed;

            await UpdateDevice(createdDevice, true);

            DeviceModel updatedDevice = await GetDeviceByMacAddress(createdDevice.MACAddress);

            Assert.True(ModelsAreEqual(createdDevice, updatedDevice));
        }

        [Theory]
        [InlineData("00:e0:4c", "Hello")]
        [InlineData("000a959d6816", "Hello")]
        [InlineData("00-0a-95-9d-68-16", "Hello")]
        [InlineData(null, "111")]
        [InlineData("", "111")]
        [InlineData("    ", "111")]
        [InlineData("00:0a:95:9d:68:16", "")]
        [InlineData("00:0a:95:9d:68:16", "    ")]
        [InlineData("00:0a:95:9d:68:16", null)]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("    ", "    ")]
        public async Task UpdateDeviceAsync_InvalidModel_DeviceIsNotUpdated(string macAddress, string name)
        {
            DeviceModel createdDevice = await GetCreatedDeviceAsync();
            createdDevice.MACAddress = macAddress;
            createdDevice.Name = name;

            await UpdateDevice(createdDevice, false);

            DeviceModel updatedDevice = await GetDeviceByMacAddress(createdDevice.MACAddress);
            Assert.False(ModelsAreEqual(createdDevice, updatedDevice));
        }

        [Fact]
        public async Task UpdateDeviceAsync_AdminCanNotUpdateDeviceWithNullValue_IsNotSuccessfulStatusAsync()
        {
            await UpdateDevice(null, false);
        }

        [Fact]
        public async Task UpdateDeviceAsync_UpdateTheSameModelWithoutChanges_ModelWasUpdated()
        {
            DeviceModel createdDevice = await GetCreatedDeviceAsync();

            await UpdateDevice(createdDevice, true);

            DeviceModel updatedDevice = await GetDeviceByMacAddress(createdDevice.MACAddress);
            Assert.True(ModelsAreEqual(createdDevice, updatedDevice));
        }

        [Fact]
        public async Task DeleteDeviceAsync_AdminCanDeleteDevice_IsSuccessfulStatus()
        {
            DeviceModel createdDevice = await GetCreatedDeviceAsync();

            HttpResponseMessage result = await AdminHttpClient.DeleteAsync(_baseUrl + createdDevice.Id);

            Assert.True(result.IsSuccessStatusCode);

            DeviceModel deletedDevice = await GetDeviceByMacAddress(createdDevice.MACAddress);

            Assert.True(deletedDevice == null);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task DeleteDeviceAsync_CanNotDeleteDeviceWithInvalidId_DeviceIsNotFound(int deviceId)
        {
            HttpResponseMessage result = await AdminHttpClient.DeleteAsync(_baseUrl + deviceId);

            Assert.False(result.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(_baseUrl + "", "Get")]
        [InlineData(_baseUrl + "1", "Get")]
        [InlineData(_baseUrl + "1/cameras", "Get")]
        [InlineData(_baseUrl + "", "Post")]
        [InlineData(_baseUrl + "1", "Delete")]
        [InlineData(_baseUrl + "1/deactivate", "Post")]
        public async Task AllAuthRequiredMethods_AnonymousCanNotGetInfo_StatusCode401(string url, string method)
        {
            HttpClient httpClient = _factory.CreateClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(method), url);

            HttpResponseMessage result = await httpClient.SendAsync(requestMessage);

            Assert.True(result.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData(_baseUrl + "", "Get")]
        [InlineData(_baseUrl + "1", "Get")]
        [InlineData(_baseUrl + "1/cameras", "Get")]
        [InlineData(_baseUrl + "", "Post")]
        [InlineData(_baseUrl + "1", "Delete")]
        [InlineData(_baseUrl + "1/deactivate", "Post")]
        public async Task AllAuthRequiredAdminRoleMethods_UserRoleCanNotGetInfo_StatusCode401(string url, string method)
        {
            HttpClient httpClient = _factory.CreateClient();
            await accountsInstanse.AuthorizeUserAsync(httpClient);
            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(method), url);

            HttpResponseMessage result = await httpClient.SendAsync(requestMessage);

            Assert.True(result.StatusCode == HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetDevicesAsync_WithFilter_MacAddressExists()
        {
            int numberOfDevices = 100;
            var devices = new List<DeviceModel>();
            for (int i = 0; i < numberOfDevices; i++) {
                DeviceModel createdDevice = await GetCreatedDeviceAsync();
                devices.Add (createdDevice);
            }

            string aMac = devices [numberOfDevices / 2].MACAddress;
            string filterModelUrl = $"?skip=0&take=100&search={aMac}&filterBy=1"; ;
            string url = _baseUrl + filterModelUrl;
            var foundDevices = await GetAsync <PageResponse<DeviceModel>> (url);
            var devicesArray = foundDevices.Items.ToArray(); 

            Assert.True(devicesArray.Length == 1);
            Assert.True(string.Compare(devicesArray[0].MACAddress, aMac) == 0);
        }



        #endregion
    }
}
