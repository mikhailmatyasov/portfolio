using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests;
using WeSafe.Authentication.Integration.Tests.Static;
using WeSafe.Event.WebApi;
using WeSafe.Web.Common.Authentication;

namespace WeSafe.Event.Integration.Tests
{
    public class EventBaseTest
    {
        private const string _mediaType = "application/json";
        protected string _authToken { get; set; }
        protected readonly HttpClient _client;
        private readonly HttpClient _authClient;

        public EventBaseTest(WebApplicationFactory<Startup> factory, AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup> authFactory)
        {
            _authClient = authFactory.CreateClient();
            _client = factory.CreateClient();
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
