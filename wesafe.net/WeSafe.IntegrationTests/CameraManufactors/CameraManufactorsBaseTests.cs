using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Account;
using WeSafe.IntegrationTests.Base;
using WeSafe.Services.Client.Models;
using Xunit;

namespace WeSafe.IntegrationTests.CameraManufactors
{
    public class CameraManufactorsBaseTests : BaseTest
    {
        #region Fields

        protected const string _baseUrl = "/api/cameraManufactor/";
        protected readonly AccountTests accountTestInstance;

        #endregion

        #region Ctor

        public CameraManufactorsBaseTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            accountTestInstance = new AccountTests(factory);
            Login().Wait();
        }

        #endregion

        #region PublicMethods

        public async Task<IEnumerable<CameraManufactorModel>> GetCameraManufactors()
        {
            var result = await AdminHttpClient.GetAsync(_baseUrl);

            Assert.True(result.IsSuccessStatusCode);

            IEnumerable<CameraManufactorModel> cameraManufactors = await GetParsedResponseAsync<IEnumerable<CameraManufactorModel>>(result);

            Assert.NotNull(cameraManufactors);

            return cameraManufactors;
        }

        #endregion
    }
}
