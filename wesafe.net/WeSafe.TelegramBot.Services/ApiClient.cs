using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeSafe.Services.Client.Models;

namespace WeSafe.TelegramBot.Services
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Role { get; set; }

        public DateTime ExpiresAt { get; set; }
    }

    public class ApiClient : IApiClient
    {
        private readonly ILogger<ApiClient> _logger;
        private readonly IConfiguration _configuration;
        private static TokenResponse _token;

        public ApiClient(ILogger<ApiClient> logger, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            Client = clientFactory.CreateClient("api");
        }

        protected HttpClient Client { get; }

        protected bool IsAuthenticated => _token != null && DateTime.UtcNow < _token.ExpiresAt;

        private async Task Authenticate(CancellationToken cancellationToken)
        {
            var body = JsonConvert.SerializeObject(new
            {
                UserName = _configuration.GetSection("Api")["UserName"],
                Password = _configuration.GetSection("Api")["Password"]
            });

            var response = await Client.PostAsync("/api/account/token", new StringContent(body, Encoding.UTF8, "application/json"), cancellationToken);

            response.EnsureSuccessStatusCode();

            _token = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if ( !IsAuthenticated ) await Authenticate(cancellationToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

            var response = await Client.SendAsync(request, cancellationToken);

            if ( response.StatusCode == HttpStatusCode.Unauthorized )
            {
                await Authenticate(cancellationToken);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

                response = await Client.SendAsync(request, cancellationToken);
            }

            var body = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            return response;
        }

        private Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            return SendRequestAsync(request, cancellationToken);
        }

        public async Task<TelegramUserModel> GetUserByChatId(long chatId, CancellationToken cancellationToken)
        {
            var response = await GetAsync($"/api/telegram/user/{chatId}", cancellationToken);

            return JsonConvert.DeserializeObject<TelegramUserModel>(await response.Content.ReadAsStringAsync());
        }

        public async Task<TelegramUserModel> RegisterTelegramUser(RegisterTelegramUserModel model, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/telegram/register")
            {
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json")
            };

            var response = await SendRequestAsync(request, cancellationToken);

            return JsonConvert.DeserializeObject<TelegramUserModel>(
                await response.Content.ReadAsStringAsync());
        }

        public async Task<IEnumerable<DeviceStatusModel>> GetSystemStatus(long chatId, CancellationToken cancellationToken)
        {
            var response = await GetAsync($"/api/telegram/systemstatus/{chatId}", cancellationToken);

            return JsonConvert.DeserializeObject<IEnumerable<DeviceStatusModel>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserSettingsModel> GetUserSettings(long chatId, CancellationToken cancellationToken)
        {
            var response = await GetAsync($"/api/telegram/settings/{chatId}", cancellationToken);

            return JsonConvert.DeserializeObject<UserSettingsModel>(await response.Content.ReadAsStringAsync());
        }

        public async Task<IEnumerable<CameraModel>> GetDeviceCameras(int deviceId, CancellationToken cancellationToken)
        {
            var response = await GetAsync($"/api/devices/{deviceId}/cameras?active=true", cancellationToken);

            return JsonConvert.DeserializeObject<IEnumerable<CameraModel>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<CameraModel> GetCameraById(int cameraId, CancellationToken cancellationToken)
        {
            var response = await GetAsync($"/api/cameras/{cameraId}", cancellationToken);

            return JsonConvert.DeserializeObject<CameraModel>(await response.Content.ReadAsStringAsync());
        }

        public async Task SaveCamera(CameraModel camera, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "/api/cameras")
            {
                Content = new StringContent(JsonConvert.SerializeObject(camera), Encoding.UTF8, "application/json")
            };

            var response = await SendRequestAsync(request, cancellationToken);
        }

        public async Task MuteSystem(long chatId, DateTimeOffset? mute, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/telegram/mute")
            {
                Content = new StringContent(JsonConvert.SerializeObject(new TelegramMuteModel
                {
                    TelegramId = chatId,
                    Mute = mute
                }), Encoding.UTF8, "application/json")
            };

            var response = await SendRequestAsync(request, cancellationToken);
        }

        public async Task ArmDevice(int deviceId, bool arm, CancellationToken cancellationToken)
        {
            var response = await GetAsync($"/api/devices/{deviceId}", cancellationToken);

            var device = JsonConvert.DeserializeObject<DeviceModel>(await response.Content.ReadAsStringAsync());

            if ( device == null ) throw new Exception("Device not found");

            device.IsArmed = arm;

            var request = new HttpRequestMessage(HttpMethod.Put, "/api/devices")
            {
                Content = new StringContent(JsonConvert.SerializeObject(device), Encoding.UTF8, "application/json")
            };

            response = await SendRequestAsync(request, cancellationToken);
        }

        public async Task MuteCamera(long chatId, CameraSettingsModel model, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/telegram/mutecamera/{chatId}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json")
            };

            var response = await SendRequestAsync(request, cancellationToken);
        }
    }
}