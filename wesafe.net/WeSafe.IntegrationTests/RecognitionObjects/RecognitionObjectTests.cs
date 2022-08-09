using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeSafe.IntegrationTests.Base;
using WeSafe.Services.Client.Models;
using Xunit;

namespace WeSafe.IntegrationTests.RecognitionObjects
{
    public class RecognitionObjectTests : BaseTest
    {
        #region Fields

        HttpClient httpClient;
        protected const string _baseUrl = "/api/RecognitionObjects/";

        #endregion

        #region Ctor

        public RecognitionObjectTests(MediaGalleryFactory<StubStartup> factory) : base(factory)
        {
            Login().Wait();
            httpClient = _factory.CreateClient();
        }

        #endregion

        [Fact]
        public async Task CreateRecognitionObjectAsync_AdminCanCreateValidModel_ModelIsCreatedAsync()
        {
            RecognitionObjectModel model = GetValidRecognitionObjectModel();

            var result = await AdminHttpClient.PostAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

            var content = await result.Content.ReadAsStringAsync();

            Assert.True(result.IsSuccessStatusCode);

            var ro = await GetRecognitionObjectByNameAsync(model.Name);

            Assert.NotNull(ro);
        }

        [Fact]
        public async Task CreateRecognitionObjectAsync_AdminCreateDuplicateModel_IsNotSuccessfulStatus()
        {
            var model = await GetCreatedRecognitionObjectAsync();

            var result = await AdminHttpClient.PostAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

            var content = await result.Content.ReadAsStringAsync();

            Assert.False(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task UpdateRecognitionObjectAsync_AdminUpdateModel_ModelIsUpdated()
        {
            var model = await GetCreatedRecognitionObjectAsync();

            model.IsActive = false;

            var result = await AdminHttpClient.PutAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

            var content = await result.Content.ReadAsStringAsync();

            Assert.True(result.IsSuccessStatusCode);

            var ro = await GetRecognitionObjectByNameAsync(model.Name);

            Assert.NotNull(ro);
            Assert.Equal(model.IsActive, ro.IsActive);
        }

        [Fact]
        public async Task UpdateRecognitionObjectAsync_AdminUpdateModelName_ModelIsNotUpdated()
        {
            var model = await GetCreatedRecognitionObjectAsync();
            string expectedName = model.Name;

            model.Name = GetRandomString();

            var result = await AdminHttpClient.PutAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

            var content = await result.Content.ReadAsStringAsync();

            Assert.True(result.IsSuccessStatusCode);

            var notFound = await GetRecognitionObjectByNameAsync(model.Name);
            var found = await GetRecognitionObjectByNameAsync(expectedName);

            Assert.Null(notFound);
            Assert.NotNull(found);
            Assert.Equal(expectedName, found.Name);
        }

        [Fact]
        public async Task GetRecognitionObjectByIdAsync_AdminCanGetRecognitionObjectById_ModelIsFoundAsync()
        {
            var model = await GetCreatedRecognitionObjectAsync();

            var found = await GetAsync<RecognitionObjectModel>(_baseUrl + model.Id);

            Assert.True(ModelsAreEqual(model, found));
        }

        private RecognitionObjectModel GetValidRecognitionObjectModel()
        {
            return new RecognitionObjectModel
            {
                Name = GetRandomString(),
                Description = GetRandomString(),
                IsActive = true
            };
        }

        private async Task<RecognitionObjectModel> GetRecognitionObjectByNameAsync(string name)
        {
            var objects = await GetAllRecognitionObjects();

            Assert.False(objects.Count(u => u.Name == name) > 1);

            return objects.SingleOrDefault(u => u.Name == name);
        }

        private async Task<IEnumerable<RecognitionObjectModel>> GetAllRecognitionObjects()
        {
            var result = await GetAsync<IEnumerable<RecognitionObjectModel>>(_baseUrl);

            Assert.NotNull(result);

            return result;
        }

        private async Task<RecognitionObjectModel> GetCreatedRecognitionObjectAsync()
        {
            var  model = GetValidRecognitionObjectModel();

            await AdminHttpClient.PostAsync(_baseUrl, new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

            return await GetRecognitionObjectByNameAsync(model.Name);
        }
    }
}