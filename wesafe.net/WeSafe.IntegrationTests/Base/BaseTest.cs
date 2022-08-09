using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.Services.Client.Models;
using WeSafe.Web.Core.Models;
using Xunit;

namespace WeSafe.IntegrationTests.Base
{
    public class BaseTest : IClassFixture<MediaGalleryFactory<StubStartup>>
    {
        public WebApplicationFactory<StubStartup> _factory { get; }
        public HttpClient AdminHttpClient { get; }
        public HttpClient UnauthorizedHttpClient { get; }
        public HttpClient UserHttpClient { get; }

        public BaseTest(MediaGalleryFactory<StubStartup> factory)
        {
            _factory = factory;
            AdminHttpClient = _factory.CreateClient();
            UnauthorizedHttpClient = _factory.CreateClient();
        }

        public async Task Login()
        {
            LoginModel model = new LoginModel
            {
                UserName = "admin",
                Password = "123456"
            };
            var result = await AdminHttpClient.PostAsync("/api/account/token", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
            TokenResponse tokenResponse = await GetParsedResponseAsync<TokenResponse>(result);

            AdminHttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenResponse.AccessToken);
        }

        public async Task LoginUser(HttpClient httpClient, LoginModel loginModel)
        {
            var result = await httpClient.PostAsync("/api/account/token", new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json"));
            TokenResponse tokenResponse = await GetParsedResponseAsync<TokenResponse>(result);

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenResponse.AccessToken);
        }

        public string GetRandomString()
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder randomString = new StringBuilder(10);

            for (int i = 0; i < 8; i++)
            {
                randomString.Append(chars[random.Next(chars.Length)]);
            }

            return randomString.ToString();
        }

        public string GetRandomPhoneNumber()
        {
            Random random = new Random();
            string chars = "0123456789";
            StringBuilder randomString = new StringBuilder("+", 12);

            for (int i = 0; i < 12; i++)
            {
                randomString.Append(chars[random.Next(chars.Length)]);
            }

            return randomString.ToString();
        }

        public string GetRandomIpAddress()
        {
            var random = new Random();
            return $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}";
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            var result = await AdminHttpClient.GetAsync(url);

            var parsedResponse = await GetParsedResponseAsync<T>(result);

            return parsedResponse;
        }

        protected async Task<T> GetParsedResponseAsync<T>(HttpResponseMessage result)
        {
            var response = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(response);
        }

        protected bool ModelsAreEqual<T>(T firstModel, T secondModel) where T : class =>
            JsonConvert.SerializeObject(firstModel) == JsonConvert.SerializeObject(secondModel);
    }
}
