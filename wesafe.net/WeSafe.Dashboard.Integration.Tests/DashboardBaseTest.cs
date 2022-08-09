using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests;
using WeSafe.Authentication.Integration.Tests.Static;
using WeSafe.Dashboard.WebApi;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Dashboard.Integration.Tests
{
    public class DashboardBaseTest
    {
        private const string _mediaType = "application/json";
        private string _authToken { get; set; }
        private readonly HttpClient _client;
        private readonly HttpClient _authClient;

        public DashboardBaseTest(WebApplicationFactory<Startup> factory, AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup> authFactory)
        {
            _authClient = authFactory.CreateClient();
            _client = factory.CreateClient();
        }

        public async Task<T> Post<T>(string url, object content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content),
                    Encoding.UTF8, _mediaType)
            };

            if (!string.IsNullOrWhiteSpace(_authToken))
                request.Headers.Add("Authorization", $"Bearer {_authToken}");

            var result = await _client.SendAsync(request);

            var response = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(response);
        }

        public async Task Post(string url, object content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content),
                    Encoding.UTF8, _mediaType)
            };

            if (!string.IsNullOrWhiteSpace(_authToken))
                request.Headers.Add("Authorization", $"Bearer {_authToken}");

            var result = await _client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            result.EnsureSuccessStatusCode();
        }

        public async Task<T> Get<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrWhiteSpace(_authToken))
                request.Headers.Add("Authorization", $"Bearer {_authToken}");

            var result = await _client.SendAsync(request);

            var response = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(response);
        }

        public async Task Authorize(string username = "user", string password = "123456")
        {
            var request = new HttpRequestMessage(HttpMethod.Post, AuthenticationUrls.loginUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { Username = username, Password = password }),
                    Encoding.UTF8, _mediaType)
            };

            var result = await _authClient.SendAsync(request);

            var response = await result.Content.ReadAsStringAsync();
            var result1 = JsonConvert.DeserializeObject<TokenResponseModel>(response);

            _authToken = result1.AccessToken;
        }
    }
}
