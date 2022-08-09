using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WeSafe.IntegrationTests.Base;
using WeSafe.Services.Client.Models;
using Xunit;

namespace WeSafe.IntegrationTests.CameraManufactors
{
    public class CameraManufactorsTests : CameraManufactorsBaseTests
    {
        #region Ctor

        public CameraManufactorsTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
        }

        #endregion

        #region Tests

        [Fact]
        public async Task GetCameraManufactors_AdminCanGetCameraManufactors_NotNullCollectionAsync()
        {
            var result = await AdminHttpClient.GetAsync(_baseUrl);

            Assert.True(result.IsSuccessStatusCode);

            IEnumerable<CameraManufactorModel> cameraManufactors = await GetParsedResponseAsync<IEnumerable<CameraManufactorModel>>(result);

            Assert.NotNull(cameraManufactors);
        }

        [Fact]
        public async Task GetCameraManufactors_UserCanGetCameraManufactors_NotNullCollectionAsync()
        {
            HttpClient httpClient = _factory.CreateClient();
            await accountTestInstance.AuthorizeUserAsync(httpClient);

            var result = await httpClient.GetAsync(_baseUrl);

            Assert.True(result.IsSuccessStatusCode);

            IEnumerable<CameraManufactorModel> cameraManufactors = await GetParsedResponseAsync<IEnumerable<CameraManufactorModel>>(result);

            Assert.NotNull(cameraManufactors);
        }

        [Fact]
        public async Task GetCameraManufactors_AnonymousCanNotGetCameraManufactors_IsNotSuccessfulStatusAsync()
        {
            HttpClient httpClient = _factory.CreateClient();

            var result = await httpClient.GetAsync(_baseUrl);

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);

            IEnumerable<CameraManufactorModel> cameraManufactors = await GetParsedResponseAsync<IEnumerable<CameraManufactorModel>>(result);

            Assert.Null(cameraManufactors);
        }

        #endregion
    }
}