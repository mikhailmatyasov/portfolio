using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests.Static;
using WeSafe.Authentication.WebApi.Models;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Authentication.Integration.Tests
{
    public class BaseTest<TStartup> where TStartup : class
    {
        private const string _mediaType = "application/json";
        private string _authToken { get; set; }
        private readonly HttpClient _client;

        public BaseTest(WebApplicationFactory<TStartup> factory)
        {
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
                request.Headers.Add("Authentication", $"Bearer {_authToken}");

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
                request.Headers.Add("Authentication", $"Bearer {_authToken}");

            var result = await _client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            result.EnsureSuccessStatusCode();
        }

        public async Task<T> Get<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrWhiteSpace(_authToken))
                request.Headers.Add("Authentication", $"Bearer {_authToken}");

            var result = await _client.SendAsync(request);

            var response = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(response);
        }

        public async Task Authorize(string username = "user", string password = "123456")
        {
            var result = await Post<TokenResponseModel>(AuthenticationUrls.loginUrl,
                new { UserName = username, Password = password });

            _authToken = result.AccessToken;
        }
    }
}
