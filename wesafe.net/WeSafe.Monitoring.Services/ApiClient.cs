using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeSafe.Services.Client.Models;

namespace WeSafe.Monitoring.Services
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

        public async Task<IEnumerable<DeviceShortModel>> GetDevices(bool activatedOnly, string status, CancellationToken cancellationToken = default)
        {
            var url = $"/api/monitoring/alldevices?activatedOnly={activatedOnly}";

            if ( !String.IsNullOrWhiteSpace(status) ) url += $"&status={status}";

            var response = await GetAsync(url, cancellationToken);

            return JsonConvert.DeserializeObject<IEnumerable<DeviceShortModel>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<IEnumerable<CameraMonitoringModel>> GetDeviceCameras(int deviceId, DateTimeOffset? timeMark = null, bool activeOnly = false, CancellationToken cancellationToken = default)
        {
            var url = $"/api/monitoring/cameras?deviceId={deviceId}&activeOnly={activeOnly}";

            if ( timeMark.HasValue ) url += $"&timeMark={HttpUtility.UrlEncode(timeMark.Value.ToString("O"))}";

            var response = await GetAsync(url, cancellationToken);

            return JsonConvert.DeserializeObject<IEnumerable<CameraMonitoringModel>>(await response.Content.ReadAsStringAsync());
        }

        public async Task UpdateDeviceStatus(DeviceUpdateStatusModel model, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/monitoring/devicestatus")
            {
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json")
            };

            var response = await SendRequestAsync(request, cancellationToken);
        }

        public async Task UpdateCameraStatus(CameraUpdateStatusModel model, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/monitoring/camerastatus")
            {
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json")
            };

            var response = await SendRequestAsync(request, cancellationToken);
        }

        public async Task StatusChanged(DeviceUpdateStatusModel model, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/monitoring/statuschanged")
            {
                Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json")
            };

            var response = await SendRequestAsync(request, cancellationToken);
        }
    }
}