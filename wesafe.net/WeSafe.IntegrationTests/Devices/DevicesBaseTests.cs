using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Base;
using WeSafe.Services.Client.Models;
using WeSafe.Shared;
using WeSafe.Web.Core.Models;
using Xunit;

namespace WeSafe.IntegrationTests.Devices
{
    public class DevicesBaseTests : BaseTest
    {
        #region Fields

        protected const string _baseUrl = "/api/devices/";

        #endregion

        #region Ctor

        public DevicesBaseTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            Login().Wait();
        }

        #endregion

        #region Public Methods

        public async Task<DeviceModel> GetCreatedDeviceAsync()
        {
            string macAddress = GenerateValidMacAddress();
            await AddValidDeviceAsync(macAddress);

            return await GetDeviceByMacAddress(macAddress);
        }

        public async Task<DeviceTokenModel> GetValidDeviceTokenModelAsync()
        {
            DeviceModel createdDevice = await GetCreatedDeviceAsync();
            return new DeviceTokenModel
            {
                DeviceToken = createdDevice.Token
            };
        }

        public async Task<IEnumerable<DeviceModel>> GetAllDevices()
        {
            var result = await GetAsync<PageResponse<DeviceModel>>(_baseUrl);

            Assert.NotNull(result);
            Assert.NotNull(result.Items);

            return result.Items;
        }

        public async Task<DeviceModel> GetDeviceByMacAddress(string macAddress)
        {
            IEnumerable<DeviceModel> devices = await GetAllDevices();

            return devices.SingleOrDefault(x => x.MACAddress == macAddress);
        }

        public async Task<HttpResponseMessage> AddDeviceAsync(DeviceModel deviceModel)
        {
            var rs = await AdminHttpClient.PostAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(deviceModel), Encoding.UTF8, "application/json"));
            var content = await rs.Content.ReadAsStringAsync();
            return rs;
        }

        public DeviceModel CreateDeviceModel(string macAddress)
        {
            return new DeviceModel
            {
                MACAddress = macAddress,
                Name = "Name",
                CurrentSshPassword = "password",
                MaxActiveCameras = 4
            };
        }

        public string GenerateValidMacAddress()
        {
            Random random = new Random();
            byte[] buffer = new byte[6];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => $"{x:X2}:").ToArray());

            return result.TrimEnd(':');
        }

        #endregion

        #region Protected 

        protected async Task<HttpResponseMessage> AddValidDeviceAsync(string macAddress)
        {
            DeviceModel deviceModel = CreateDeviceModel(macAddress);

            var result = await AddDeviceAsync(deviceModel);

            Assert.True(result.IsSuccessStatusCode);

            return result;
        }

        protected async Task UpdateDevice(DeviceModel device, bool isSucceeded)
        {
            var result = await AdminHttpClient.PutAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(device), Encoding.UTF8, "application/json"));

            if (isSucceeded)
                Assert.True(result.IsSuccessStatusCode);
            else
                Assert.False(result.IsSuccessStatusCode);
        }

        #endregion
    }
}