using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Base;
using WeSafe.Services.Client.Models;
using Xunit;

namespace WeSafe.IntegrationTests.PermittedAdminIp
{
    public class PermittedAdminIpTests : BaseTest
    {
        #region Fields

        protected const string _baseUrl = "/api/permittedAdminIp/";

        #endregion

        #region Ctor

        public PermittedAdminIpTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            Login().Wait();
        }

        #endregion

        #region Tests

        [Fact]
        public async Task CreatePermittedAdminIpAsync_AdminCanCreateValidIp_IpIsCreatedAsync()
        {
            PermittedAdminIpModel permittedAdminIp = GetPermittedAdminIpModel(GetRandomIpAddress());

            await CreateIp(permittedAdminIp, true);

            PermittedAdminIpModel foundIp = await GetIpAddressByIpAsync(permittedAdminIp.Ip);

            Assert.NotNull(foundIp);
        }

        [Theory]
        [InlineData("1.1.1.")]
        [InlineData(null)]
        [InlineData("dsgdsfedd")]
        public async Task CreatePermittedAdminIpAsync_AdminCanNotCreateInvalidIp_IpIsNotCreatedAsync(string ip)
        {
            PermittedAdminIpModel permittedAdminIp = GetPermittedAdminIpModel(ip);

            await CreateIp(permittedAdminIp, false);

            PermittedAdminIpModel foundIp = await GetIpAddressByIpAsync(ip);

            Assert.Null(foundIp);
        }

        [Fact]
        public async Task CreatePermittedAdminIpAsync_AdminCanNotCreateNulldIp_IpIsNotCreatedAsync()
        {
            await CreateIp(null, false);

            PermittedAdminIpModel foundIp = await GetIpAddressByIpAsync(null);

            Assert.Null(foundIp);
        }

        [Fact]
        public async Task RemovePermittedAdminIpSync_AdminCanDeleteIp_IpIsDeletedAsync()
        {
            PermittedAdminIpModel permittedAdminIp = GetPermittedAdminIpModel(GetRandomIpAddress());
            await CreateIp(permittedAdminIp, true);
            PermittedAdminIpModel createdIp = await GetIpAddressByIpAsync(permittedAdminIp.Ip);

            var result = await AdminHttpClient.DeleteAsync(_baseUrl + createdIp.Id);

            var content = await result.Content.ReadAsStringAsync();
            Assert.True(result.IsSuccessStatusCode);

            PermittedAdminIpModel foundIp = await GetIpAddressByIpAsync(permittedAdminIp.Ip);

            Assert.Null(foundIp);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task RemovePermittedAdminIpSync_AdminCanNotDeleteIpViaInvalidIp_IpNotSuccessfulStatusAsync(int ipId)
        {
            var result = await AdminHttpClient.DeleteAsync(_baseUrl + ipId);

            Assert.False(result.IsSuccessStatusCode);
        }

        #endregion

        #region PublicMethods

        #endregion

        #region PrivateMethods

        private async Task<IEnumerable<PermittedAdminIpModel>> GetAllIps()
        {
            var result = await GetAsync<IEnumerable<PermittedAdminIpModel>>(_baseUrl);

            Assert.NotNull(result);

            return result;
        }

        private async Task<PermittedAdminIpModel> GetIpAddressByIpAsync(string ip)
        {
            IEnumerable<PermittedAdminIpModel> ips = await GetAllIps();

            return ips.FirstOrDefault(i => i.Ip == ip);
        }



        private async Task CreateIp(PermittedAdminIpModel permittedAdminIp, bool isSucceeded)
        {
            var result = await AdminHttpClient.PostAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(permittedAdminIp), Encoding.UTF8, "application/json"));

            var content = await result.Content.ReadAsStringAsync();

            if (isSucceeded)
                Assert.True(result.IsSuccessStatusCode);
            else
                Assert.False(result.IsSuccessStatusCode);
        }

        private PermittedAdminIpModel GetPermittedAdminIpModel(string ip)
        {
            return new PermittedAdminIpModel
            {
                Ip =ip
            };
        }
      
        #endregion
    }   
}
