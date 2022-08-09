using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Account;
using WeSafe.IntegrationTests.Base;
using WeSafe.IntegrationTests.CameraManufactors;
using WeSafe.Services.Client.Models;
using Xunit;

namespace WeSafe.IntegrationTests.RtspPath
{
    public class RtspPathTests : BaseTest
    {
        #region Fields

        protected const string _baseUrl = "/api/rtspPath/";
        private readonly AccountTests accountTestInstance;
        private readonly CameraManufactorsBaseTests cameraManufactorBaseInstance;

        #endregion

        #region Ctor

        public RtspPathTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            accountTestInstance = new AccountTests(factory);
            cameraManufactorBaseInstance = new CameraManufactorsBaseTests(factory);
            Login().Wait();
        }

        #endregion

        #region Tests

        [Fact]
        public async Task GetRtspPaths_AdminCanGetRtspPaths_NotNullCollectionAsync()
        {
            CameraMarkModel cameraMark = await GetCameraMarkAsync();

            var result = await AdminHttpClient.GetAsync(_baseUrl + cameraMark.Id);

            Assert.True(result.IsSuccessStatusCode);

            IEnumerable<RtspPathModel> rtspPaths = await GetParsedResponseAsync<IEnumerable<RtspPathModel>>(result);

            Assert.True(ModelsAreEqual(cameraMark.RtspPaths, rtspPaths));
        }

        [Fact]
        public async Task GetRtspPaths_UserCanGetRtspPaths_NotNullCollectionAsync()
        {
            CameraMarkModel cameraMark = await GetCameraMarkAsync();
            HttpClient httpClient = _factory.CreateClient();
            await accountTestInstance.AuthorizeUserAsync(httpClient);

            var result = await httpClient.GetAsync(_baseUrl + cameraMark.Id);

            Assert.True(result.IsSuccessStatusCode);

            IEnumerable<RtspPathModel> rtspPaths = await GetParsedResponseAsync<IEnumerable<RtspPathModel>>(result);

            Assert.True(ModelsAreEqual(cameraMark.RtspPaths, rtspPaths));
        }

        [Fact]
        public async Task GetRtspPaths_AnonymousCanNotGetRtspPaths_IsNotSuccessfulStatusAsync()
        {
            HttpClient httpClient = _factory.CreateClient();

            var result = await httpClient.GetAsync(_baseUrl + "1");

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);

            IEnumerable<RtspPathModel> rtspPaths = await GetParsedResponseAsync<IEnumerable<RtspPathModel>>(result);

            Assert.Null(rtspPaths);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task GetRtspPaths_AdminCanNotGetRtspPathsViaNotExistingCameraMarkId_IsNotSuccessfulStatusAsync(int cameraMarkId)
        {
            var result = await AdminHttpClient.GetAsync(_baseUrl + cameraMarkId);
           
            Assert.False(result.IsSuccessStatusCode);
        }

        #endregion

        #region PrivateMethods

        private async Task<CameraMarkModel> GetCameraMarkAsync()
        {
            IEnumerable<CameraManufactorModel> cameraManufactors = await cameraManufactorBaseInstance.GetCameraManufactors();
            CameraMarkModel cameraMark = cameraManufactors.FirstOrDefault().CameraMarks.FirstOrDefault();

            Assert.NotNull(cameraMark);

            return cameraMark;
        }

        #endregion        
    }
}
