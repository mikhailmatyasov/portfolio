using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WeSafe.Authentication.Integration.Tests;
using WeSafe.Event.WebApi;
using Xunit;

namespace WeSafe.Event.Integration.Tests.AddEvent
{
    public class AddEventTests : EventBaseTest, IClassFixture<EventWebApplicationFactory<Startup>>,
        IClassFixture<AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup>>
    {
        private const string _url = "/api/events";
        public AddEventTests(EventWebApplicationFactory<Startup> factory,
            AuthenticationWebApplicationFactory<WeSafe.Authentication.WebApi.Startup> authFactory) : base(factory,
            authFactory)
        {
        }

        [Fact]
        public async Task AddEvent_ValidParams_Passed()
        {
            await Authorize();

            var fileStream = await File.ReadAllBytesAsync("FormFile.png");

            var content = GetMultipartFormDataContent("62:44:d1:91:91:cc", "1", fileStream);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _url);
            httpRequestMessage.Headers.Add("Authorization", "Bearer " + _authToken);
            httpRequestMessage.Content = content;

            var res = await _client.SendAsync(httpRequestMessage);

            Assert.True(res.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("123", "255.255.255.255")]
        [InlineData("62:44:d1:91:91:cc", "500.255.255.255")]
        [InlineData("123", "800.255.255.255")]
        public async Task AddEvent_InValidParams_ErrorWith400StatusCode(string mac, string cameraId)
        {
            await Authorize();

            var fileStream = await File.ReadAllBytesAsync("FormFile.png");

            var content = GetMultipartFormDataContent(mac, cameraId, fileStream);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _url);
            httpRequestMessage.Headers.Add("Authorization", "Bearer " + _authToken);
            httpRequestMessage.Content = content;

            var res = await _client.SendAsync(httpRequestMessage);

            Assert.True(res.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddEvent_UserNotAuthorized_401ErrorCode()
        {
            var fileStream = await File.ReadAllBytesAsync("FormFile.png");

            var content = GetMultipartFormDataContent("62:44:d1:91:91:cc", "1", fileStream);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _url);
            httpRequestMessage.Content = content;

            var res = await _client.SendAsync(httpRequestMessage);

            Assert.True(res.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AddEvent_FramesIsEmpty_400ErrorCode()
        {
            await Authorize();

            var content = GetMultipartFormDataContent("62:44:d1:91:91:cc", "1", new byte[]{});

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _url);
            httpRequestMessage.Headers.Add("Authorization", "Bearer " + _authToken);
            httpRequestMessage.Content = content;

            var res = await _client.SendAsync(httpRequestMessage);

            Assert.True(res.StatusCode == HttpStatusCode.BadRequest);
        }

        private MultipartFormDataContent GetMultipartFormDataContent(string mac, string cameraId, byte[] fileStream)
        {
            return new MultipartFormDataContent
            {
                {new StringContent(mac), "DeviceMacAddress"},
                {new StringContent(cameraId), "CameraId"},
                {new ByteArrayContent(fileStream), "Frames", "image.png"},
            };
        }
    }
}
